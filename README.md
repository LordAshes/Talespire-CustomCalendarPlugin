# Custom Calendar Plugin

This unofficial TaleSpire tracks months, days and optionally hours and minutes with a banner displaying the date and
optional time under the tool bar.

This plugin, like all others, is free but if you want to donate, use: http://198.91.243.185/TalespireDonate/Donate.php

## Change Log

```
1.2.0: Banner line is now a full line centered so longer entries can be displayed. Banner file is always 1920.
       Adjust the width of the displayed banner to suit desired length.
1.2.0: Optimization to read banner texture only once. This also avoids multiple FindFile entries in the log.
1.1.0: Switch from BoardPersistence to ChatService. DateTime now game wide common to all boards.
1.0.1: Bug fix to avoid exception when Board does not have a Date/Time set
1.0.0: Initial release
```

## Install

Use R2ModMan or similar installer to install this plugin.

Set the desired setting, including the current date/time, using the R2ModMan config for the plugin.


## Usage

The message for the corresponding day will be displayed in the date/time banner at the top of the screen.

```
Right Shift + A = Adds one day to the current date.
Right Shift + S = Subtracts one day from the current date.
Right Shift + M = Prompts for how many minutes to add (1).
Right Shift + D = Prompts for current month, day, hour and minutes (2). 

(1) Minutes can be negative but must in both cases must be entered with a prefix sign and at least a 2 digit number.
    Use a preceeding 0 when adjusting by less than 10 minutes (e.g. +05 or -07).
	
(2) The format for setting date is Month:Day:Hour:Minute for example 02:05:08:35. Set hour and minutes to 00 if not used.
```

### Demo Assets

The Custom Calendar Plugin comes with a regular modern Western calendar. However, the calendar can be adjusted to suit your
own world. The number of minutes per hour and number of hours per day default to 60 and 24 but can be set in the R2ModMan
settings for this plugin.

The Calendar.json file defines the months and days of the month. Each entry is the message that is to be shown for that day.
The default just shows the date but you can customize these entries to show any message on any given day. If a day has an empty
message, the banner will not show for that day.

The JSON format is as follows:

[
  [
    "The 1st of January",
    "The 2nd of January",
    "The 3rd of January",
	...
	"The 31st of January"
  ],
  [
    "The 1st of February",
    "The 2nd of February",
    "The 3rd of February",
	...
    "The 28th of February"
  ],
  ...
]
   
Where ... means mode code that has been removed to make the sample smaller.

## Persistence

The date time is common to all boards to loading a new board will retain the current campaign date/time.
