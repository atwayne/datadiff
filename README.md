datadiff
========

This tool will get similar data from different sources (only SQL Server supported for now), and then exported the merged result into an excel with styles for different scenarios.

To start, create your config file and have your app.config pointing to that config file. Then you are ready to go.

config.xml
========
You can find a template of config file under the lastr2d2.Tools.DataDiff.Deploy project. I have put some comments there so you can fill in the config by following the comments.


Screenshot
----------
And here is a screenshot for the final excel:

![Screenshot](https://raw.githubusercontent.com/lastr2d2/datadiff/master/screenshot.jpg)

 - Records which are identical filled in dark green,
 - Records which are slightly different<sup>1</sup> filled in light green,
 - Records which are different filled in yellow,
 - Records which are missing from one data source filled in red,


1. Slightly different is for number columns, the definition is the difference between two numbers should be less than a pre-defined percentage. (See Gap in the config.xml)


Thanks
----------


1. Thanks to [ClosedXML - The easy way to OpenXML](http://closedxml.codeplex.com/)