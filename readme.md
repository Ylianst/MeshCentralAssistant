# MeshCentral Assistant

For more information, [visit MeshCentral.com](https://meshcentral.com).

This is a tray icon tool for the MeshCentral. It displays the state of the agent, you can start and stop the agent and request help.
If a meshagent.msh file is present in the same folder as the MeshCentralAssistant.exe executable, this tool will act as an agent on it and allow direct connection to the server.

## Command Line Switches

```
-debug                       When set a "debug.log" file will be created and internal state of MeshCentral Assistant will be written to it. This is useful for developers to debug issues.
-visible                     When starting on the system stay MeshCentral Assistant will start with the dialog box visible on the primary screen.
-noupdate                    Don't accept self-update requests from the server.
-help:"Sample help request"  When specified, MeshCentral Assistant will issue a help request to to the server upon connection.
-agentname:"Computer1"       When specified, this is the initial name that will be displayed on the server for this device.
-connect                     If not specified in the .msh file (AutoConnect value) MeshCentral Assistant will automatically connect to the server.
-noproxy                     When specified, HTTP proxies will not be used.
```

It is important to note that only one MeshCentral Assistant process can be run on a client device.  Any additional processes will be terminated immediately.  A help request specified in the command line switches will not be executed before the process is terminated.  Be sure to check for and terminate existing MeshCentral Assistant processes if an external program launches Assistant to send a help request.

## MSH Format

The .msh file contains the policy MeshCentral Assistant built-in agent will follow. It includes what server to connect to, how to authenticate the server, what device group to initially join, etc. A sample .msh file will look like this and may be included within the MeshCentral Assistant executable when downloaded from the server.

```
MeshID=0xEDBE1BE3771D69D...
ServerID=D99362D5ED8BAE...
MeshServer=wss://sample.com:443/agent.ashx
DiscoveryKey=SecretDiscoveryPassword
AutoConnect=3
Title=Compagny Name
Image=iVBORw0KGgoAAAANSUhEU...
```

Here is a list of the possible keys that are currently supported by the agent. Note that the key name must have the exact capitalization:

```
MeshID                       The unique identifier of the device group to initially join.
ServerID                     The hash of the server agent certificate, this is used to securely authenticate the server.
MeshServer                   The URL of the server to connect to, if this value is set to "local" then multicast on the local network is used to find the server.
DiscoveryKey                 If multicast is used to find the server on the local network and this key is specified
AutoConnect                  This is a binary flag value. 1 = Don't be on the system tray, 2 = Auto-connect to the server. Use 3 to specify both flags, 0 for no flags.
Title                        When specified, this is a branded title used for the MeshCentral Asssitant dialog box.
Image                        A base64 encoded PNG file with a branded logo to use for the MeshCentral Asssitant dialog box.
disableUpdate                When set to any value, this will disable MeshCentral Assistant self-update system.
ignoreProxyFile              When set to any value, HTTP proxies will not be used.
```

## Video Tutorials
You can watch many tutorial videos on the [MeshCentral YouTube Channel](https://www.youtube.com/channel/UCJWz607A8EVlkilzcrb-GKg/videos). MeshCentral Assistant has it's own introduction video.

Introduction to MeshCentral Assistant.  
[![MeshCentral - Assistant](https://img.youtube.com/vi/HytCpYLxrvk/mqdefault.jpg)](https://www.youtube.com/watch?v=HytCpYLxrvk)

## License
This software is licensed under [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0).
