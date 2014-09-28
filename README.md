datadiff
========

This tool will get similar data from different sources, exported the merged result into an excel<sup>1<sup> with styles for different scenarios.

To start, open the lastr2d2.Tools.DataDiff.Deploy project, edit the config.xml, and F5!

----------
Here is a screenshot for the final excel:
Records which are identical filled in dark green,
Records which are slightly different<sup>2</sup> filled in light green,
Records which are different filled in yellow,
Records which are missing from one data source filled in red,

![Screenshot](https://raw.githubusercontent.com/lastr2d2/datadiff/master/screenshot.jpg)


----------


1. Thanks to [ClosedXML - The easy way to OpenXML](http://closedxml.codeplex.com/)
2. Slightly different is for number columns, the definition is the difference between two numbers should be less than a pre-defined percentage. (See Gap in the config.xml)