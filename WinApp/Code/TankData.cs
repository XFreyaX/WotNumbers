﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Code
{
	public static class TankData
	{
		#region DatabaseLookup

		public static DataTable tankList = new DataTable();

		public static void GetTankListFromDB()
		{
			tankList.Clear();
			tankList = DB.FetchData("SELECT * FROM tank");
			foreach (DataRow dr in tankList.Rows)
			{
				// Replace WoT API tank name with Phalynx Dossier tank name
				string tankName = dr["name"].ToString();
				tankName = tankName.Replace("ö", "o");
				tankName = tankName.Replace("ä", "a");
				tankName = tankName.Replace("â", "a");
				tankName = tankName.Replace("ß", "ss");
				dr["name"] = tankName;
				dr.AcceptChanges();
			}
			tankList.AcceptChanges();
		}

		public static DataTable GetPlayerTankFromDB(int tankId)
		{
			string sql = "SELECT * FROM playerTank WHERE playerId = " + Config.Settings.playerId + " AND tankId=" + tankId.ToString();
			return DB.FetchData(sql);
		}

		public static DataTable GetPlayerTankBattleFromDB(int playerTankId, string battleMode)
		{
			string sql = "SELECT * FROM playerTankBattle WHERE playerTankId=" + playerTankId.ToString() + " AND battleMode='" + battleMode + "'";
			return DB.FetchData(sql);
		}


		public static int GetPlayerTankCount()
		{
			string sql = "SELECT count(id) AS count FROM playerTank WHERE playerId = " + Config.Settings.playerId;
			DataTable dt = DB.FetchData(sql);
			int count = 0;
			if (dt.Rows.Count > 0) count = Convert.ToInt32(dt.Rows[0]["count"]);
			return count;
		}

		public static int ConvertWs2TankId(int wsTankId, int wsCountryId)
		{
			string sql = "SELECT tankId " +
						 "FROM wsTankId " +
						 "WHERE wsTankId = " + wsTankId.ToString() + " AND wsCountryId = " + wsCountryId.ToString();
			DataTable dt = DB.FetchData(sql);
			int lookupTankId = 0;
			if (dt.Rows.Count > 0) lookupTankId = Convert.ToInt32(dt.Rows[0]["tankId"]);
			return lookupTankId;
		}


		public static int GetPlayerTankId(int tankId)
		{
			string sql = "SELECT playerTank.id AS playerTankId " +
						 "FROM playerTank INNER JOIN tank ON playerTank.tankid = tank.id " +
						 "WHERE tank.id = " + tankId;
			DataTable dt = DB.FetchData(sql);
			int lookupTankId = 0;
			if (dt.Rows.Count > 0) lookupTankId = Convert.ToInt32(dt.Rows[0]["playerTankId"]);
			return lookupTankId;
		}

		public static DataTable GetBattleFromDB(int battleId)
		{
			string sql = "SELECT * FROM battle WHERE id=" + battleId.ToString();
			return DB.FetchData(sql);
		}

		public static int GetBattleIdForImportedWsBattleFromDB(int wsId)
		{
			string sql = "SELECT Id FROM battle WHERE wsId=" + wsId.ToString();
			DataTable dt = DB.FetchData(sql);
			int lookupBattle = 0;
			if (dt.Rows.Count > 0) lookupBattle = Convert.ToInt32(dt.Rows[0]["Id"]);
			return (lookupBattle);
		}

		public static DataTable json2dbMapping = new DataTable();
		
		public static void GetJson2dbMappingFromDB()
		{
			json2dbMapping.Clear();
			json2dbMapping = DB.FetchData("SELECT * FROM json2dbMapping ORDER BY jsonMainSubProperty");
		}

		public static DataTable GetTankData2BattleMappingFromDB(string battleMode)
		{
			string sql =
				"SELECT  dbDataType, dbPlayerTank, dbPlayerTankMode, dbBattle " +
				"FROM    dbo.json2dbMapping " +
				"WHERE   (dbBattle IS NOT NULL) AND (dbPlayerTankMode IS NULL OR dbPlayerTankMode=@dbPlayerTankMode) " +
				"GROUP BY dbDataType, dbPlayerTank, dbBattle, dbPlayerTankMode ";
			DB.AddWithValue(ref sql, "@dbPlayerTankMode", battleMode, DB.SqlDataType.VarChar);
			return DB.FetchData(sql);
		}

		#endregion

		#region LookupData

		// TODO: just for testing
		public static string ListTanks()
		{
			string s = "";
			foreach (DataRow dr in tankList.Rows)
			{
				s += dr["id"] + ":" + dr["name"] + ", ";
			}
			return s;
		}

		public static int GetTankID(string TankName)
		{
			int tankID = 0;
			string expression = "name = '" + TankName + "'";
			DataRow[] foundRows = tankList.Select(expression);
			if (foundRows.Length > 0) // If tank exist in Tank table 
			{
				tankID = Convert.ToInt32(foundRows[0]["id"]);
			}
			return tankID;
		}


		public static int GetTankID(string TankName, out int TankTier)
		{
			int tankID = 0;
			TankTier = 0;
			string expression = "name = '" + TankName + "'";
			DataRow[] foundRows = tankList.Select(expression);
			if (foundRows.Length > 0) // If tank exist in Tank table 
			{
				tankID = Convert.ToInt32(foundRows[0]["id"]);
				TankTier = Convert.ToInt32(foundRows[0]["tier"]);
			}
			return tankID;
		}

		public static bool TankExist(int tankID)
		{
			string expression = "id = " + tankID.ToString();
			DataRow[] foundRows = tankList.Select(expression);
			return (foundRows.Length > 0);
		}

		public static DataRow TankInfo(int tankID)
		{
			string expression = "id = " + tankID.ToString();
			DataRow[] foundRows = tankList.Select(expression);
			if (foundRows.Length > 0)
				return foundRows[0];
			else
				return null;
		}

		//public static void SetPlayerTankAllAch()
		//{
		//	// This makes sure all player tanks has all achievmenets - default value count=0
		//	string sql = "insert into playerTankAch (playerTankId, achId, achCount) " +
		//				"select playerTankAchAllView.playerTankId, playerTankAchAllView.achId, 0 from playerTankAchAllView left join " +
		//				"playerTankAch on playerTankAchAllView.playerTankId = playerTankAch.playerTankId and playerTankAchAllView.achId = playerTankAch.achId " +
		//				"where playerTankAch.playerTankId is null";
		//	DB.ExecuteNonQuery(sql);
		//}

		//public static void SetPlayerTankAllAch(int playerTankId)
		//{
		//	// This makes sure this player tanks has all achievmenets - default value count=0
		//	string sql = "insert into playerTankAch (playerTankId, achId, achCount) " +
		//				"select " + playerTankId.ToString() + ", ach.id, 0 from ach left join " +
		//				"playerTankAch on ach.id = playerTankAch.achId and playerTankAch.playerTankId = " + playerTankId.ToString() + " " +
		//				"where playerTankAch.playerTankId is null";
		//	DB.ExecuteNonQuery(sql);
		//}


		public static bool GetAchievmentExist(string achName)
		{
			bool exists = false;
			string sql = "SELECT ach.id " +
							"FROM ach  " +
							"WHERE name = '" + achName + "'";
			DataTable dt = DB.FetchData(sql);
			exists = (dt.Rows.Count > 0);
			return exists;
		}

		
		#endregion

	   
	}
}