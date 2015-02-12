# Arduino-Lync-monitor
This is a complete demo project to connect an external arduino board (with a buzzer and a 3-color led) to Microsoft Lync.
 - If lync status is available, the led is turned into green
 - If lync status is busy, the led is turned into red
 - If lync status is away/offline..., the led is turned into yellow (mix of green and red)
 - If lync status is "do not disturb", the led is turned into dark red (mix of red and blue)

The result is the following: https://www.youtube.com/watch?v=WJlGYOmVJPo

The solution contains three projects:
 - CommandMessenger: Library used for Aruino-2-PC and PC-2-Arduino communications
 - ArduinoLyncNotifier: .NET C# WPF application that monitors Microsoft Lync and notifies Arduino board about availability changes and incoming calls
 - ArduinoSketch: Arduino code to handle events sent by ArduinoLyncNotifier app
 
ArduinoLyncNotifier contains also a simulation mode that enables you to test Arduino board without having Lync installed (it just sends desired messages to Arduino)

Special thanks to:
 - CmdMessenger (https://github.com/thijse/Arduino-Libraries/tree/master/CmdMessenger)
 - http://www.princetronics.com/supermariothemesong/ for the code used to play SuperMario song with a buzzer.
 
 
 Hardware:
  - Arduino Uno REV 3
  - Three resistors
  - One 3-colors led (connected to pin 9, 10, 11 through resistors)
  - A buzzer (connected to pin 6)
  - Optionally: A variable resistor (connected to pin A0) used to set music speed
  - USB cable to connect pc to arduino board.
  
Software:
 - Visual Studio 2013
 - Arduino sofware: http://arduino.cc/en/Main/Software
 - Optionally: Arduino studio for VS2013 (http://www.visualmicro.com/)
