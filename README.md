# About ChromaProject
Windows Core Api visualisation using Razer Chroma written in C#.

I started this project mostly to get more familiar with a couple of things:

* Using WPF (Windows Presentation Foundation) with C#.
* Working with DLL and COM in C#.
* Working with more hardware related API's (This project uses the Colore API for controlling the lights on Chroma keyboards).
* Working with more specific C# related conventions.

I had alot of fun learning about these techniques (especially COM), though they were a bit confusing at first, and would love
to help others in learning about it too. That's why I provided as much documentation as required in the project itself on utilising COM.

# Usage
Start visualisation by pressing the big buttons stating "start visualising audio" (to state the obvious, I know) /n
These variables will alter the output depending on whats been set:

## Visualisation Data
* From Speakers: This will cause the program to retrieve it's data from the default connected speakers.
* From Microphone: This will cause the program to retrieve it's data from the default connected microphone.
----
* Static Color: Will cause the keyboard to be lit in only one color, currently that's green.
* Ramp Color: Will cause the keyboard to be lit based off of a ramp defined in the ColorPicker class.
* Random Color: Will kayse the keyboard to be lit with a random color.

## Keyboard Update Interval
This is the interval in miliseconds that happens between every update of the keyboard, adjust the slider to see
its effect.

## Multiply By
Incoming audio will be multiplied by this factor, for sake of ease I added a threshold of 0.05 near the 1 value.

If the slider value falls within that range audio will not be amplified yet. (the value should also display "None")

# Documentation
Documentation is provided in the sourcecode as comments where I felt it would be most helpful. I've also provided some extra examples in AudioUtils
on how to retrieve COM objects.

# Future improvements
If you have an idea or cool feature with which I (or you, I won't mind a pull request ;) ) could improve this project
feel free to open an issue on github.

# External Sources & Special thanks
Resources:  
[COM](https://nl.wikipedia.org/wiki/Component_Object_Model)  
[DLL](https://nl.wikipedia.org/wiki/Dynamic-link_library)  
[Colore](https://github.com/chroma-sdk/Colore)  

Other libraries about Windows Core Audio API:  
[Naudio](https://github.com/naudio/NAudio)  
[Coreaudio-dotnet](https://github.com/ThiefMaster/coreaudio-dotnet)  
[Cscore](https://github.com/filoe/cscore)  
I didn't use these 3 libraries directly but they did provide a lot of very useful examples :)

Special thanks to
[Simon Mourier](https://github.com/smourier) and [Sverrir Sigmnudarson](https://github.com/sverrirs)
for providing some excelent example on how to use the Windows Core Audio api in your C# projects.