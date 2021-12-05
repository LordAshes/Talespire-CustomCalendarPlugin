using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace LordAshes
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(LordAshes.FileAccessPlugin.Guid)]
    public partial class CustomCalendarPlugin : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "Custom Calendar Plug-In";
        public const string Guid = "org.lordashes.plugins.customcalendar";
        public const string Version = "1.2.0.0";

        // Configuration
        private ConfigEntry<KeyboardShortcut> advanceDayTriggerKey { get; set; }
        private ConfigEntry<KeyboardShortcut> receedDayTriggerKey { get; set; }
        private ConfigEntry<KeyboardShortcut> advanceMinutesTriggerKey { get; set; }
        private ConfigEntry<KeyboardShortcut> setDateTimeTriggerKey { get; set; }

        public static bool displayTime = true;
        public static int minutesPerHour = 60;
        public static int hoursPerDay = 24;
        public static ConfigEntry<int> currentMinute;
        public static ConfigEntry<int> currentHour;
        public static ConfigEntry<int> currentDay;
        public static ConfigEntry<int> currentMonth;

        public static Texture2D bannerTexture = FileAccessPlugin.Image.LoadTexture("Banner.png");

        GUIStyle guiStyle = new GUIStyle();

        List<List<string>> calendar = JsonConvert.DeserializeObject<List<List<string>>>(FileAccessPlugin.File.ReadAllText("Calendar.json"));

        /// <summary>
        /// Function for initializing plugin
        /// This function is called once by TaleSpire
        /// </summary>
        void Awake()
        {
            // Not required but good idea to log this state for troubleshooting purpose
            UnityEngine.Debug.Log("Custom Calendar Plugin: Active.");

            // The Config.Bind() format is category name, setting text, default
            displayTime = Config.Bind("Settings", "Display Time", true).Value;
            minutesPerHour = Config.Bind("Settings", "Minutes Per Hour", 60).Value;
            hoursPerDay = Config.Bind("Settings", "Hours Per Day", 24).Value;
            advanceMinutesTriggerKey = Config.Bind("Hotkeys", "Adjust Minutes", new KeyboardShortcut(KeyCode.M, KeyCode.RightShift));
            advanceDayTriggerKey = Config.Bind("Hotkeys", "Add Day", new KeyboardShortcut(KeyCode.A, KeyCode.RightShift));
            receedDayTriggerKey = Config.Bind("Hotkeys", "Subtract Day", new KeyboardShortcut(KeyCode.S, KeyCode.RightShift));
            setDateTimeTriggerKey = Config.Bind("Hotkeys", "Set Date And Time", new KeyboardShortcut(KeyCode.D, KeyCode.RightShift));

            guiStyle.alignment = UnityEngine.TextAnchor.MiddleCenter;
            guiStyle.normal.textColor = UnityEngine.Color.black;
            guiStyle.fontSize = Config.Bind("Hotkeys", "Display Font Size", 16).Value;
            guiStyle.fontStyle = FontStyle.Bold;

            currentMonth = Config.Bind("Settings", "Current Month", 1);
            currentDay = Config.Bind("Settings", "Current Day", 1);
            currentHour = Config.Bind("Settings", "Current Hour", 0);
            currentMinute = Config.Bind("Settings", "Current Minute", 0);

            ChatServicePlugin.handlers.Add("/campaigndate", ChatHandler);

            Utility.PostOnMainPage(this.GetType());
        }

        /// <summary>
        /// Function for determining if view mode has been toggled and, if so, activating or deactivating Character View mode.
        /// This function is called periodically by TaleSpire.
        /// </summary>
        void Update()
        {
            if (Utility.isBoardLoaded())
            {
                if (Utility.StrictKeyCheck(advanceMinutesTriggerKey.Value))
                {
                    SystemMessage.AskForTextInput("Time Adjustment", "Adjust Minutes By (e.g. +05):", "OK", (strMinutes) =>
                    {
                        int intMinutes = 0;
                        if (int.TryParse(strMinutes, out intMinutes))
                        {
                            currentMinute.Value = currentMinute.Value + intMinutes;
                        }
                        ConformAndDistributeCalendar();
                    }, null, "Cancel", null, "+60");
                }
                if (Utility.StrictKeyCheck(setDateTimeTriggerKey.Value))
                {
                    SystemMessage.AskForTextInput("Time Adjustment", "Specify The Month:Date:Hour:Minute (e.g. 05:10:30:00):", "OK", (strDateTime) =>
                    {
                        SetCalendar(strDateTime);
                        ConformAndDistributeCalendar();
                    }, null, "Cancel", null, "00:00:00:00");
                }
                if (Utility.StrictKeyCheck(advanceDayTriggerKey.Value))
                { 
                    currentDay.Value++;
                    currentHour.Value = 0;
                    currentMinute.Value = 0;
                    ConformAndDistributeCalendar();
                }
                if (Utility.StrictKeyCheck(receedDayTriggerKey.Value)) 
                { 
                    currentDay.Value--; 
                    currentHour.Value = 0; 
                    currentMinute.Value = 0;
                    ConformAndDistributeCalendar();
                }
            }
        }

        void OnGUI()
        {
            if (Utility.isBoardLoaded())
            {
                try
                {
                    if (calendar.ElementAt(currentMonth.Value - 1).ElementAt(currentDay.Value - 1) != "")
                    {
                        GUI.DrawTexture(new Rect(0, 60, 1920, 47), bannerTexture);
                        if (displayTime)
                        {
                            GUI.Label(new Rect(0, 62, 1920, 45), calendar.ElementAt(currentMonth.Value - 1).ElementAt(currentDay.Value - 1) + " " + currentHour.Value.ToString("d2") + ":" + currentMinute.Value.ToString("d2"), guiStyle);
                        }
                        else
                        {
                            GUI.Label(new Rect(0, 62, 1920, 45), calendar.ElementAt(currentMonth.Value - 1).ElementAt(currentDay.Value - 1), guiStyle);
                        }
                    }
                }
                catch(Exception)
                {
                    Debug.Log("Trying To Render Date Month=" + currentMonth + " Day=" + currentDay + " Hour=" + currentHour + " Minute=" + currentMinute);
                }
            }
        }

        void ConformAndDistributeCalendar()
        {
            while (currentMinute.Value < 0) { currentMinute.Value = currentMinute.Value + minutesPerHour; currentHour.Value--; }
            while (currentMinute.Value >= minutesPerHour) { currentMinute.Value = currentMinute.Value - minutesPerHour; currentHour.Value++; }
            while (currentHour.Value < 0) { currentDay.Value--; currentHour.Value = currentHour.Value - hoursPerDay; }
            while (currentHour.Value >= hoursPerDay) { currentHour.Value = currentHour.Value - 60; currentDay.Value--; }
            while (currentDay.Value < 1) { currentMonth.Value = currentMonth.Value - 1; currentDay.Value = currentDay.Value + calendar.ElementAt(currentMonth.Value - 1).Count; }
            while (currentDay.Value > calendar.ElementAt(currentMonth.Value - 1).Count) { currentDay.Value = currentDay.Value - calendar.ElementAt(currentMonth.Value - 1).Count; currentMonth.Value++; }
            SendCalendar(CustomCalendarPlugin.currentMonth.Value, CustomCalendarPlugin.currentDay.Value, CustomCalendarPlugin.currentHour.Value, CustomCalendarPlugin.currentMinute.Value);
        }
    }
}
