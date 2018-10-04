# GildedRose
 A CLI program to automate inventory management based on the Gilded Rose rules.

# Instructions to Build
Click the Clone or download button and choose your preferred option.

a. From VS 2017: Build and use "dotnet GildedRose.dll InputFile.txt" to run (with supplied GildedRoseInput.txt)

b. From CLI Solution Directory: Build an executable using "dotnet build -r win10-x64" and use built executable

# CLI Usage
GildedRose InputFile.txt
  
Where InputFile.txt is a file containing a list of finest goods in the format: ITEMNAME | SELLIN | QUALITY  
  
A sample input file is provided in the solution.

# Unit Tests
Unit tests created with xUnit.  
  
To run tests, In VS 2017, go to Test Explorer and Run All.

# Notes
See DesignNotes.txt in GildedRose project for extra notes.  
  
See UserAcceptanceTests.txt in test project for test notes.
