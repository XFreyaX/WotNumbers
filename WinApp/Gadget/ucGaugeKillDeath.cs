﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinApp.Code;
using System.Diagnostics;
using WinApp.Gadget;
using WinApp.Code.FormLayout;

namespace WinApp.Gadget
{
	public partial class ucGaugeKillDeath : UserControl
	{
		string _battleMode = "";
		GadgetHelper.TimeRangeEnum _battleTimeSpan = GadgetHelper.TimeRangeEnum.Total;

        public ucGaugeKillDeath(string battleMode, GadgetHelper.TimeRangeEnum timeSpan)
		{
			InitializeComponent();
			_battleMode = battleMode;
            _battleTimeSpan = timeSpan;
		}

		private void ucGauge_Load(object sender, EventArgs e)
		{
			
		}

		public void DataBind()
		{
            // Colors
            aGauge1.SetColorRanges(ColorRangeScheme.RangeKillDeath);
            // show correct timespan button as selected
            switch (_battleTimeSpan)
            {
                case GadgetHelper.TimeRangeEnum.Total:
                    btnTotal.Checked = true;
                    break;
                case GadgetHelper.TimeRangeEnum.TimeMonth3:
                    btn3M.Checked = true;
                    break;
                case GadgetHelper.TimeRangeEnum.TimeMonth:
                    btnMonth.Checked = true;
                    break;
                case GadgetHelper.TimeRangeEnum.TimeWeek:
                    btnWeek.Checked = true;
                    break;
                case GadgetHelper.TimeRangeEnum.TimeToday:
                    btnToday.Checked = true;
                    break;
            }
			// Show battle mode
			string capText = "Total";
            capText = BattleMode.GetItemFromSqlName(_battleMode).Name;
			lblBattleMode.Text = capText;
			string sqlBattlemode = "";
			string sql = "";
			if (_battleTimeSpan == GadgetHelper.TimeRangeEnum.Total)
			{
				if (_battleMode != "")
				{
					sqlBattlemode = " AND (playerTankBattle.battleMode = @battleMode) ";
					DB.AddWithValue(ref sqlBattlemode, "@battleMode", _battleMode, DB.SqlDataType.VarChar);
				}
				sql =
					"SELECT SUM(playerTankBattle.frags) AS frags, SUM(playerTankBattle.battles - playerTankBattle.survived) AS kills " +
					"FROM   playerTankBattle INNER JOIN " +
					"		playerTank ON playerTankBattle.playerTankId = playerTank.id " +
					"WHERE  (playerTank.playerId = @playerId) " + sqlBattlemode;
			}
			else
			{
				// Create Battle Time filer
				DateTime dateFilter = DateTimeHelper.GetTodayDateTimeStart(); 
				// Adjust time scale according to selected filter
				switch (_battleTimeSpan)
				{
					case GadgetHelper.TimeRangeEnum.TimeWeek:
						dateFilter = dateFilter.AddDays(-7);
						break;
					case GadgetHelper.TimeRangeEnum.TimeMonth:
						dateFilter = dateFilter.AddMonths(-1);
						break;
					case GadgetHelper.TimeRangeEnum.TimeMonth3:
						dateFilter = dateFilter.AddMonths(-3);
						break;
				}
				if (_battleMode != "")
				{
					sqlBattlemode = " AND (battle.battleMode = @battleMode) ";
					DB.AddWithValue(ref sqlBattlemode, "@battleMode", _battleMode, DB.SqlDataType.VarChar);
				}
				sql =
					"SELECT SUM(battle.frags) AS frags, SUM(battle.killed) AS kills " +
					"FROM   battle INNER JOIN " +
					"       playerTank ON battle.playerTankId = playerTank.id " +
					"WHERE  (battle.battleTime >= @battleTime) AND (playerTank.playerId = @playerId) " + sqlBattlemode;
				DB.AddWithValue(ref sql, "@battleTime", dateFilter, DB.SqlDataType.DateTime);
			}
			DB.AddWithValue(ref sql, "@playerId", Config.Settings.playerId, DB.SqlDataType.Int);
			DataTable dt = DB.FetchData(sql);
			double frags = 0;
			double kills = 0;
			end_val = 0;
			if (dt.Rows.Count > 0 && dt.Rows[0]["frags"] != DBNull.Value)
			{
				frags = Convert.ToDouble(dt.Rows[0]["frags"]);
				kills = Convert.ToDouble(dt.Rows[0]["kills"]);
                if (kills > 0)
                    end_val = Math.Round((frags / kills), 2);
                else if (frags > 0)
                    end_val = 999;
			}
			lblLeft.Text = frags.ToString("N0");
			lblRight.Text = kills.ToString("N0");
			lblCenter.Text = end_val.ToString();
            lblCenter.ForeColor = ColorRangeScheme.KillDeathColor(end_val);
			avg_step_val = (end_val - aGauge1.ValueMin) / step_tot; // Define average movements per timer tick
			move_speed = Math.Abs(end_val - aGauge1.Value) / 30;
			timer1.Enabled = true;
		}

		
		private void btnTime_Click(object sender, EventArgs e)
		{
			BadButton b = (BadButton)sender;
			btn3M.Checked = false;
			btnMonth.Checked = false;
			btnToday.Checked = false;
			btnTotal.Checked = false;
			btnWeek.Checked = false;
			b.Checked = true;
			switch (b.Name)
			{
				case "btnTotal": _battleTimeSpan = GadgetHelper.TimeRangeEnum.Total; break;
				case "btn3M": _battleTimeSpan = GadgetHelper.TimeRangeEnum.TimeMonth3; break;
				case "btnMonth": _battleTimeSpan = GadgetHelper.TimeRangeEnum.TimeMonth; break;
				case "btnWeek": _battleTimeSpan = GadgetHelper.TimeRangeEnum.TimeWeek; break;
				case "btnToday": _battleTimeSpan = GadgetHelper.TimeRangeEnum.TimeToday; break;
			}
			DataBind();
		}

		private void ucGauge_Paint(object sender, PaintEventArgs e)
		{
			if (BackColor == ColorTheme.FormBackSelectedGadget)
				GadgetHelper.DrawBorderOnGadget(sender, e);
		}

		double move_speed = 0.05;
		double avg_step_val = 0;
		double end_val = 0;
		double step_tot = 75;
		double step_count = 0;
		bool moveNeedle = false;

		private void timer1_Tick(object sender, EventArgs e)
		{
			double gaugeVal = 0;
			if (moveNeedle)
			{
				gaugeVal = aGauge1.Value;
				if (end_val < aGauge1.Value)
				{
					gaugeVal -= move_speed;
					if (gaugeVal <= end_val || gaugeVal <= aGauge1.ValueMin)
					{
						gaugeVal = end_val;
						timer1.Enabled = false;
					}
				}
				else
				{
					gaugeVal += move_speed;
					if (gaugeVal >= end_val || gaugeVal >= aGauge1.ValueMax)
					{
						gaugeVal = end_val;
						timer1.Enabled = false;
					}
				}
				if (Math.Abs(end_val - gaugeVal) / move_speed < 19 && move_speed > 0.001)
					move_speed = move_speed * 0.95;
			}
			else
			{
				step_count++;
				gaugeVal = aGauge1.ValueMin + (Math.Exp(1 - (step_count / step_tot)) * step_count * avg_step_val);
				if (step_count >= step_tot)
				{
					gaugeVal = end_val;
					timer1.Enabled = false;
					moveNeedle = true; // use normal movment after this
				}
			}
			aGauge1.Value = (float)gaugeVal;
		}
	}
}
