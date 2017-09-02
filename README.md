# FireRTC-Bot
Rebuilding it to a Discord Bot which it's purpose is to call numbers. 
## Download/Execute
I will never build it, you have to build it yourself over visual studio. I'm sorry for those who were expecting a exe/zip file
## Doesn't start
At the first start there should appear a .ini file with the name ``config.ini``, there you need to fill out information
```
[config]
botowner = [Your Discord ID]
[token]
discord = [Bot token of your bot]
```
## How can people hear what's going on?
Well the Bot is like controlling a browser for automatization, you need a secondary discord account and virtual audio cables to forward the audio to discord and back to firertc.
## It crashes.
Well the best solution is to copy the error information shown in the console (screenshot), and create a new issue here. I will try to fix that bug.
## How can I add soundboard sounds?
Firstly, only ``.wav`` files can be played, so you need to convert them if they aren't ``.wav``.
Secondly, these files must be in the ``media\`` folder.
Thirdly, because of this audio setup you need to forward the audio from this program to both sides. A tool named Audio Router can fix this.
