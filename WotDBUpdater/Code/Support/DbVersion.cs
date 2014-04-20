﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WotDBUpdater.Code
{
	class DBVersion
	{
		// The current databaseversion
		public static int ExpectedNumber = 1; // <--------------------------------------- REMEMBER TO ADD DB VERSION NUMBER HERE - AND SUPPLY SQL SCRIPT BELOW

		// The upgrade scripts
		private static string UpgradeSQL(int version, ConfigData.dbType dbType)
		{
			// first define sqlscript for both mssql and sqlite for all versions
			string mssql = "";
			string sqlite = "";
			switch (version)
			{
				case 1: 
					break; // First version, no script
				case 2:                                                
					mssql  = "";
					sqlite = "";
					break;
				default:
					break;
			}
			string sql = "";
			// get sql for correct dbtype
			if (dbType == ConfigData.dbType.MSSQLserver) 
				sql = mssql;
			else if (dbType == ConfigData.dbType.SQLite) 
				sql = sqlite;
			// return sql
			return sql;
		}

		// Procedure upgrading DB to latest version
		public static bool CheckForDbUpgrade()
		{
			bool upgradeOK = true;
			int DBVersionCurrentNumber = CurrentNumber(); // Get current DB version
			if (DBVersionCurrentNumber == 0) return false; // Quit verison check if no db version could be found
			if (DBVersionCurrentNumber == ExpectedNumber) return true; // Quit version check when expected version is found, everything is OK!
			if (DBVersionCurrentNumber < ExpectedNumber)
			{
				// Loop through all versions missing, as long as current db version < expected version
				bool continueNext = true;
				while (DBVersionCurrentNumber < ExpectedNumber && continueNext)
				{
					// Move to next upgrade number
					DBVersionCurrentNumber++;
					// Upgrade to next db version now
					string sql = UpgradeSQL(DBVersionCurrentNumber, Config.Settings.databaseType); // Get upgrade script for this version and dbType 
					continueNext = DB.ExecuteNonQuery(sql); // Run upgrade script
					// Update db _version_ if success
					if (continueNext)
					{
						sql = "update _version_ set version=" + DBVersionCurrentNumber.ToString();
						continueNext = DB.ExecuteNonQuery(sql);
					}
				}
				// If anything went wrong (continueNext == false), supply error notification here
				if (!continueNext)
					Code.MsgBox.Show("Error occured during database upgrade, failed running SQL script for version: " + DBVersionCurrentNumber.ToString("0000"), "Error Upgrading Database");
				upgradeOK = continueNext;
				
			}
			return upgradeOK;
		}

		// Returns database current version, on first run version table is created and version = 1
		public static int CurrentNumber()
		{
			int version = 0;
			string sql = "";
			bool versionTableFound = false;
			// List tables
			DataTable dt = DB.ListTables();
			if (dt.Rows.Count > 0)
			{
				// Check if _version_ table containing db version number exists
				foreach (DataRow dr in dt.Rows)
				{
					if (dr["TABLE_NAME"].ToString() == "_version_")
					{
						versionTableFound = true;
						break;
					}
				}
				// if _version_ table not exist create it
				if (!versionTableFound)
				{
					if (Config.Settings.databaseType == ConfigData.dbType.SQLite)
						sql = "create table _version_ (id integer primary key, version integer not null); ";
					else if (Config.Settings.databaseType == ConfigData.dbType.MSSQLserver)
						sql = "create table _version_ (id int primary key, version int not null); ";
					bool createTableOK = DB.ExecuteNonQuery(sql); // Create _version_ table now
					if (!createTableOK)
						return 0; // Error occured creating _version_ table
					else
					{
						// Add initial version
						sql = "insert into _version_ (id, version) values (1,1); ";
						bool insertVersionOK = DB.ExecuteNonQuery(sql);
						if (!insertVersionOK)
							return 0; // Error occured inserting version number in _version_ table
					}
				}
				// Get version now
				sql = "select version from _version_ where id=1; ";
				dt.Dispose();
				dt = DB.FetchData(sql);
				if (dt.Rows.Count > 0)
				{
					version = Convert.ToInt32(dt.Rows[0][0]);
				}
			}
			return version;
		}
	}
}