﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizHawk.Emulation.Common;
using System.ComponentModel;
using BizHawk.Common;

namespace BizHawk.Emulation.Cores.WonderSwan
{
	partial class WonderSwan : ISettable<WonderSwan.Settings, WonderSwan.SyncSettings>
	{
		Settings _Settings;
		SyncSettings _SyncSettings;

		public class Settings
		{
			[DisplayName("Background Layer")]
			[Description("True to display the selected layer.")]
			[DefaultValue(true)]
			public bool EnableBG { get; set; }

			[DisplayName("Foreground Layer")]
			[Description("True to display the selected layer.")]
			[DefaultValue(true)]
			public bool EnableFG { get; set; }

			[DisplayName("Sprites Layer")]
			[Description("True to display the selected layer.")]
			[DefaultValue(true)]
			public bool EnableSprites { get; set; }

			public BizSwan.Settings GetNativeSettings()
			{
				var ret = new BizSwan.Settings();
				if (EnableBG) ret.LayerMask |= BizSwan.LayerFlags.BG;
				if (EnableFG) ret.LayerMask |= BizSwan.LayerFlags.FG;
				if (EnableSprites) ret.LayerMask |= BizSwan.LayerFlags.Sprite;
				return ret;
			}

			public Settings()
			{
				SettingsUtil.SetDefaultValues(this);
			}

			public Settings Clone()
			{
				return (Settings)MemberwiseClone();
			}
		}

		public class SyncSettings
		{
			[DisplayName("Initial Time")]
			[Description("Initial time of emulation.  Only relevant when UseRealTime is false.")]
			[DefaultValue(typeof(DateTime), "2010-01-01")]
			public DateTime InitialTime { get; set; }

			[Description("Your birthdate.  Stored in EEPROM and used by some games.")]
			[DefaultValue(typeof(DateTime), "1968-05-13")]
			public DateTime BirthDate { get; set; }

			[Description("True to emulate a color system.")]
			[DefaultValue(true)]
			public bool Color { get; set; }

			[DisplayName("Use RealTime")]
			[Description("If true, RTC clock will be based off of real time instead of emulated time.  Ignored (set to false) when recording a movie.")]
			[DefaultValue(false)]
			public bool UseRealTime { get; set; }

			[Description("Your gender.  Stored in EEPROM and used by some games.")]
			[DefaultValue(BizSwan.Gender.Female)]
			public BizSwan.Gender Gender { get; set; }

			[Description("Language to play games in.  Most games ignore this.")]
			[DefaultValue(BizSwan.Language.Japanese)]
			public BizSwan.Language Language { get; set; }

			[DisplayName("Blood Type")]
			[Description("Your blood type.  Stored in EEPROM and used by some games.")]
			[DefaultValue(BizSwan.Bloodtype.AB)]
			public BizSwan.Bloodtype BloodType { get; set; }

			[Description("Your name.  Stored in EEPROM and used by some games.  Maximum of 16 characters")]
			[DefaultValue("Lady Ashelia")]
			public string Name { get; set; }

			public BizSwan.SyncSettings GetNativeSettings()
			{
				var ret = new BizSwan.SyncSettings
				{
					color = Color,
					userealtime = UseRealTime,
					sex = Gender,
					language = Language,
					blood = BloodType
				};
				ret.SetName(Name);
				ret.bday = (uint)BirthDate.Day;
				ret.bmonth = (uint)BirthDate.Month;
				ret.byear = (uint)BirthDate.Year;
				ret.initialtime = (ulong)((InitialTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
				return ret;
			}

			public SyncSettings()
			{
				SettingsUtil.SetDefaultValues(this);
			}

			public SyncSettings Clone()
			{
				return (SyncSettings)MemberwiseClone();
			}

			public static bool NeedsReboot(SyncSettings x, SyncSettings y)
			{
				return !DeepEquality.DeepEquals(x, y);
			}
		}

		public Settings GetSettings()
		{
			return _Settings.Clone();
		}

		public SyncSettings GetSyncSettings()
		{
			return _SyncSettings.Clone();
		}

		public bool PutSettings(Settings o)
		{
			_Settings = o;
			var native = _Settings.GetNativeSettings();
			BizSwan.bizswan_putsettings(Core, ref native);
			return false;
		}

		public bool PutSyncSettings(SyncSettings o)
		{
			bool ret = SyncSettings.NeedsReboot(o, _SyncSettings);
			_SyncSettings = o;
			return ret;
		}

	}
}
