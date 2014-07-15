﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Common;
using System.Data.SQLite;

namespace WinAdmin
{
	public partial class GetTankDataFromAPI : Form
	{
		#region variables

		private static int itemCount;
		private static JToken rootToken;
		private static JToken itemToken;
		private static int itemId;
		private static string insertSql;
		private static string updateSql;
		//private static bool ok = true;
		//private static DataTable itemsInDB;

		// private static List<string> log = new List<string>();
		//private static string logAddedItems;
		//private static int logAddedItemsCount;
		//private static string logItemExists;
		// private static int logItemExistsCount;
		//private static string logTanksWithoutDetails; // some special tanks have lacking vehicle details in API

		private enum WotApiType
		{
			Tank = 1,
			Turret = 2,
			Gun = 3,
			Radio = 4,
			Achievement = 5,
			TankDetails = 6
		}

		#endregion
		
		public GetTankDataFromAPI()
		{
			InitializeComponent();
		}

		private void GetTankDataFromAPI_Load(object sender, EventArgs e)
		{
			
		}

		private void GetTankDataFromAPI_Shown(object sender, EventArgs e)
		{
			Refresh();
			ImportTanks();
			ImportImgLinks();
		}


		#region fetchFromAPI

		private static string FetchFromAPI(WotApiType WotAPi, int tankId)
		{
			try
			{
				string url = "";
				if (WotAPi == WotApiType.Tank)
				{
					url = "https://api.worldoftanks.eu/wot/encyclopedia/tanks/?application_id=0a7f2eb79dce0dd45df9b8fedfed7530";
				}
				if (WotAPi == WotApiType.Turret)
				{
					url = "https://api.worldoftanks.eu/wot/encyclopedia/tankturrets/?application_id=0a7f2eb79dce0dd45df9b8fedfed7530";
					// itemsInDB = DB.FetchData("select id from modTurret");   // Fetch id of turrets already existing in db
				}
				else if (WotAPi == WotApiType.Gun)
				{
					url = "https://api.worldoftanks.eu/wot/encyclopedia/tankguns/?application_id=0a7f2eb79dce0dd45df9b8fedfed7530";
					// itemsInDB = DB.FetchData("select id from modGun");   // Fetch id of guns already existing in db
				}
				else if (WotAPi == WotApiType.Radio)
				{
					url = "https://api.worldoftanks.eu/wot/encyclopedia/tankradios/?application_id=0a7f2eb79dce0dd45df9b8fedfed7530";
					// itemsInDB = DB.FetchData("select id from modRadio");   // Fetch id of radios already existing in db
				}
				else if (WotAPi == WotApiType.Achievement)
				{
					url = "https://api.worldoftanks.eu/wot/encyclopedia/achievements/?application_id=0a7f2eb79dce0dd45df9b8fedfed7530";
				}
				else if (WotAPi == WotApiType.TankDetails)
				{
					url = "https://api.worldoftanks.eu/wot/encyclopedia/tankinfo/?application_id=0a7f2eb79dce0dd45df9b8fedfed7530&tank_id=" + tankId;
				}
				HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
				httpRequest.Timeout = 10000;     // 10 secs
				httpRequest.UserAgent = "Code Sample Web Client";
				HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
				StreamReader responseStream = new StreamReader(webResponse.GetResponseStream());
				string s = responseStream.ReadToEnd();
				responseStream.Close();
				webResponse.Close();
				return s;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Could not connect to WoT API, please check your Internet access." + Environment.NewLine + Environment.NewLine +
					ex.Message, "Problem connecting to WoT API");
				return "";
			}
			
		}

		#endregion

		#region importTanks

		private void ImportTanks()
		{
			lblStatus.Text = "Getting json data from Wot API (" + DateTime.Now.ToString() + ")";
			Application.DoEvents();
			string json = FetchFromAPI(WotApiType.Tank, 0);
			if (json == "")
			{
				MessageBox.Show("No data imported, no json result from WoT API.","Error");
			}
			else
			{
				bool tankExists = false;
				lblStatus.Text = "Start checking tanks (" + DateTime.Now.ToString() + ")";
				Application.DoEvents();
				try
				{
					JObject allTokens = JObject.Parse(json);
					rootToken = allTokens.First;   // returns status token

					if (((JProperty)rootToken).Name.ToString() == "status" && ((JProperty)rootToken).Value.ToString() == "ok")
					{
						
						rootToken = rootToken.Next;
						itemCount = (int)((JProperty)rootToken).Value;   // returns count (not in use for now)
						pbStatus.Maximum = itemCount * 3; // Tree part impot, first tanks, them link to img, then download img
						rootToken = rootToken.Next;   // start reading tanks
						JToken tanks = rootToken.Children().First();   // read all tokens in data token

						//List<string> logtext = new List<string>();
						string sqlTotal = "";
						DB.DBResult result = new DB.DBResult();
						foreach (JProperty tank in tanks)   // tank = tankId + child tokens
						{
							itemToken = tank.First();   // First() returns only child tokens of tank

							itemId = Int32.Parse(((JProperty)itemToken.Parent).Name);   // step back to parent to fetch the isolated tankId
							string name = itemToken["name_i18n"].ToString();

							// Write to db
							string sql = "select 1 from tank where id=@id";
							DB.AddWithValue(ref sql, "@id", itemId, DB.SqlDataType.Int, Settings.Config);
							DataTable dt = DB.FetchData(sql, Settings.Config, out result);
							tankExists = (dt.Rows.Count > 0);
							
							insertSql = "INSERT INTO tank (id, name) VALUES (@id, @name); ";
							updateSql = "UPDATE tank set name=@name WHERE id=@id; ";

							// insert if tank does not exist
							if (!tankExists)
							{
								lblStatus.Text = "Insert new tank: " + name;
								pbStatus.Value++;
								Application.DoEvents();
								DB.AddWithValue(ref insertSql, "@id", itemId, DB.SqlDataType.Int, Settings.Config);
								DB.AddWithValue(ref insertSql, "@name", name, DB.SqlDataType.VarChar, Settings.Config);
								sqlTotal += insertSql + Environment.NewLine;
							}

							// update if tank exists
							else
							{
								lblStatus.Text = "Updating tank: " + name;
								pbStatus.Value++;
								Application.DoEvents();
								DB.AddWithValue(ref updateSql, "@id", itemId, DB.SqlDataType.Int, Settings.Config);
								DB.AddWithValue(ref updateSql, "@name", name, DB.SqlDataType.VarChar, Settings.Config);
								sqlTotal += updateSql + Environment.NewLine;
							}
						}
						lblStatus.Text = "Saving to DB";
						Application.DoEvents();
						DB.ExecuteNonQuery(sqlTotal, Settings.Config, out result, true); // Run all SQL in batch
					}

					lblStatus.Text = "Tank id and name import complete";
					Application.DoEvents();
				}

				catch (Exception ex)
				{
					MessageBox.Show ("Import error occured: " + Environment.NewLine + Environment.NewLine + ex.Message, "Error");
				}
			}
		}

		#endregion

		#region ImportImgLinks

		private void ImportImgLinks()  // run time = 4 min
		{
			lblStatus.Text = "Getting json data from Wot API (" + DateTime.Now.ToString() + ")";
			Application.DoEvents();
			
			DB.DBResult result = new DB.DBResult();
			DataTable dtTanks = DB.FetchData("select id from tank", Settings.Config, out result);   // Fetch id of tanks in db
			int currentTank = 0;
			while (currentTank < dtTanks.Rows.Count)
			{
				int tankId = Convert.ToInt32(dtTanks.Rows[currentTank]["id"]);
				lblStatus.Text = "Getting json tank details from Wot API for tank: " + tankId.ToString();
				Application.DoEvents();
				string json = FetchFromAPI(WotApiType.TankDetails, tankId);
				if (json == "")
				{
					lblStatus.Text = "No data imported, no json result from WoT API.";
				}
				else
				{
					try
					{
						JObject allTokens = JObject.Parse(json);
						rootToken = allTokens.First;   // returns status token

						if (((JProperty)rootToken).Name.ToString() == "status" && ((JProperty)rootToken).Value.ToString() == "ok")
						{
							rootToken = rootToken.Next;
							itemCount = (int)((JProperty)rootToken).Value;   // returns count (not in use for now)

							rootToken = rootToken.Next;   // start reading tanks
							JToken tanks = rootToken.Children().First();   // read all tokens in data token

							foreach (JProperty tank in tanks)   // tank = tankId + child tokens
							{
								itemToken = tank.First();   // First() returns only child tokens of tank

								itemId = Int32.Parse(((JProperty)itemToken.Parent).Name);   // step back to parent to fetch the isolated tankId

								if (itemToken.HasValues)  // Check if tank data exists in API (some special tanks have empty data token)
								{
									lblStatus.Text = "Retrieve tank images for tank: " + tankId.ToString();
									pbStatus.Value++;
									Application.DoEvents();
									System.Threading.Thread.Sleep(300);  // Limited to 2-4 API lookups per second. Function spends 1 sek per tank as is, so no need for delay atm.
									
									JArray tanksArray = (JArray)itemToken["tanks"];  // fail
									string imgPath = itemToken["image"].ToString();
									string smallImgPath = itemToken["image_small"].ToString();
									string contourImgPath = itemToken["contour_image"].ToString();
									string tankName = itemToken["name_i18n"].ToString();
									updateSql = "UPDATE tank set imgPath=@imgPath, smallImgPath=@smallImgPath, contourImgPath=@contourImgPath WHERE id=@id";
									DB.AddWithValue(ref updateSql, "@id", itemId, DB.SqlDataType.Int, Settings.Config);
									DB.AddWithValue(ref updateSql, "@imgPath", imgPath, DB.SqlDataType.VarChar, Settings.Config);
									DB.AddWithValue(ref updateSql, "@smallImgPath", smallImgPath, DB.SqlDataType.VarChar, Settings.Config);
									DB.AddWithValue(ref updateSql, "@contourImgPath", contourImgPath, DB.SqlDataType.VarChar, Settings.Config);
									DB.ExecuteNonQuery(updateSql, Settings.Config, out result);

									lblStatus.Text = "Downloading images for tank: " + tankId.ToString();
									pbStatus.Value++;
									Application.DoEvents();
									byte[] img = getImageFromAPI(imgPath);
									byte[] smallImg = getImageFromAPI(smallImgPath);
									byte[] contourImg = getImageFromAPI(contourImgPath);
									
									// SQL Lite binary insert
									string conString = Config.DatabaseConnection(Settings.Config);
									SQLiteConnection con = new SQLiteConnection(conString);
									SQLiteCommand cmd = con.CreateCommand();
									cmd.CommandText = "UPDATE tank SET img=@img, smallImg=@smallImg, contourImg=@contourImg WHERE id=@id";
									SQLiteParameter imgParam = new SQLiteParameter("@img", System.Data.DbType.Binary);
									SQLiteParameter smallImgParam = new SQLiteParameter("@smallImg", System.Data.DbType.Binary);
									SQLiteParameter contourImgParam = new SQLiteParameter("@contourImg", System.Data.DbType.Binary);
									SQLiteParameter idParam = new SQLiteParameter("@id", System.Data.DbType.Int32);
									imgParam.Value = img;
									smallImgParam.Value = smallImg;
									contourImgParam.Value = contourImg;
									idParam.Value = itemId;
									cmd.Parameters.Add(imgParam);
									cmd.Parameters.Add(smallImgParam);
									cmd.Parameters.Add(contourImgParam);
									cmd.Parameters.Add(idParam);
									con.Open();
									try
									{
										cmd.ExecuteNonQuery();
									}
									catch (Exception ex)
									{
										MessageBox.Show(ex.Message);
									}
									con.Close();
								}
								else
								{
									lblStatus.Text = "No values for tank";
									pbStatus.Value++;
									pbStatus.Value++;
									Application.DoEvents();
								}
								
							}
						}
					}

					catch (Exception ex)
					{
						MessageBox.Show("Import incomplete!" + Environment.NewLine + Environment.NewLine + ex.Message, "Error");
					}
				}
				// Fetch next tank
				currentTank++;
			}
			lblStatus.Text = "Tank image import complete";
			Application.DoEvents();
		}

		#endregion

		#region getImageFromAPI

		public static byte[] getImageFromAPI(string url)
		{
			byte[] imgArray;

			// Fetch image from url
			WebRequest req = WebRequest.Create(url);
			WebResponse response = req.GetResponse();
			Stream stream = response.GetResponseStream();

			// Read into memoryStream
			int dataLength = (int)response.ContentLength;
			byte[] buffer = new byte[1024];
			MemoryStream memStream = new MemoryStream();
			while (true)
			{
				int bytesRead = stream.Read(buffer, 0, buffer.Length);  //Try to read the data
				if (bytesRead == 0) break;
				memStream.Write(buffer, 0, bytesRead);  //Write the downloaded data
			}

			// Read into byte array
			Image image = Image.FromStream(memStream);
			imgArray = memStream.ToArray();

			return imgArray;
		}

		#endregion


	}
}