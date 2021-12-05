using BepInEx;
using System;
using UnityEngine;

namespace LordAshes
{
    public partial class CustomCalendarPlugin : BaseUnityPlugin
    {
        private string ChatHandler(string chatMessage, string sender, ChatServicePlugin.ChatSource source)
        {
            Debug.Log("Custom Calendar Plugin: Received Calendar Update (" + chatMessage + ")");
            SetCalendar(chatMessage.Replace("/campaigndate","").Trim());
            return null;
        }

        private void SetCalendar(string Calendar)
        {
            try
            {
                string[] parts = Calendar.Split(':');
                CustomCalendarPlugin.currentMonth.Value = int.Parse(parts[0]);
                CustomCalendarPlugin.currentDay.Value = int.Parse(parts[1]);
                CustomCalendarPlugin.currentHour.Value = int.Parse(parts[2]);
                CustomCalendarPlugin.currentMinute.Value = int.Parse(parts[3]);
                Debug.Log("Custom Calendar Plugin: Date/Time Now " + CustomCalendarPlugin.currentMonth.Value + ":" + CustomCalendarPlugin.currentDay.Value + ":" + CustomCalendarPlugin.currentHour.Value + ":" + CustomCalendarPlugin.currentMinute.Value);
            }
            catch(Exception)
            {
                Debug.Log("Custom Calendar Plugin: Error Setting Date/Time '"+Calendar+"'");
            }
            Config.Save();
        }

        private void SendCalendar(int currentMonth, int currentDay, int currentHour, int currentMinute)
        {
            Debug.Log("Custom Calendar Plugin: Broadcast Calendar Update (" + currentMonth + ":" + currentDay + ":" + currentHour + ":" + currentMinute + ")");
            ChatManager.SendChatMessage("/campaigndate " + currentMonth + ":" + currentDay + ":" + currentHour + ":" + currentMinute, LocalClient.Id.Value);
        }
    }
}
