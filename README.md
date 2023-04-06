
Complete Trajectory is a .Net Core Solution that moves a camera and takes pictures in specific positions. Goal: photograph and interpret product labels in a store gondola.

Start by watching this video to visualize the behavior of the camera: `[Link to video of the movement.]`

# HARDWARE AND SOFTWARE STACK / DESIGN DECISIONS

Multi-platform, can be tested on Windows and was designed to run on Linux (Debian / Raspberry Pi OS). Thus, being developed in .Net Core.

All three projects in the solution are .Net Core Console Applications and can be run separately, but two of them are being referenced as libraries by the third ("Percurso" = Trajectory).

Design details:

* Designed to operate with a Raspberry Pi and a proprietary, custom-made board;
 
* The proprietary board creates two serial ports in the Raspberry Pi's Linux OS, one for each stepper motor, enabling this software to control them;
 
* Each stepper motor moves the camera in one direction: up-down or left-right. The communication between this solution and the proprietary board happens by sending commands to
 the board via the serial ports. The proprietary board's firmware expects specific commands and controls the stepper motors through phisical drivers;

 * Api calls use RestSharp and perform asynchronous calls.

# STRUCTURE / PROJECTS IN THE SOLUTION

 This solution consists of three projects:

 1) Trajectory ("Percurso"):

	 * Moves the camera from the position 0 downwards while zig-zagging to the right and to the left at every gondola level (at every "floor");

	 * The camera stops in front of the position where every label is supposed to be;

	 * Calls the other two projects as libraries when apropriate (this project orchestrates the whole solution).

 2) Camera ("Camera"):

	* Takes pictures of the desired objects (the labels).

 3) Files("Arquivos"):
 
	Sends pictures to an api on Azure.

	* This cloud api processes the images to interpret the labels, resulting in what product type (SKU) was located in which position.


# USAGE

Insert the api base url in the Arquivos.cs file of the Arquivos project.

```
    public static class Arquivos
    {
        public static void Send()
        {
            // Defines request
            var client = new RestClient("[API BASE URL]/api/Upload/");
            var request = new RestRequest("[API BASE URL]/api/Upload/", Method.Post);
        }
    }
```

For testing purposes (running this project as standalone), you can add the base api url to the Program.cs file of the same project as well.
