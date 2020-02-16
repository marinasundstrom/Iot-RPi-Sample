# Blink

A playground for programs targeting .NET Core 3.1 on Raspberry Pi, running Rasbian.

Based on the ```dotnet/iot``` bindings.

Containing:

* RGB LED
* RFID reader

### Requirements
* .NET Core 3.1 SDK
*  Visual Studio 2019 or VS Code
*  SSH

## Setup Raspberry Pi

1. Download Raspbian Lite from [here](https://www.raspberrypi.org/downloads/raspbian/)
2. Flash it onto a SD card with a tool like [balenaEtcher](https://www.balena.io/etcher/)

### Headless setup

To make a headless setup:

1. Mount the SD card to your computer
2. To enable SSH, create an empty file ```ssh``` in ```/boot``` partition.

```bash
touch /Volumes/boot/ssh #macOS
```

3. To set up Wi-Fi, create a file ```wpa_supplicant.conf``` in ```/boot``` with content:

```
country=US
ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev
update_config=1

network={
    ssid="NETWORK-NAME"
    psk="NETWORK-PASSWORD"
}
```

Substitute ```ssid``` and ```psk``` with the details of your network configuration.

4. Eject the SD card and mount it to the Raspberry Pi.
5. Start the RPi

### Set up Raspbian

Open a terminal (or console):

```bash
ssh pi@raspberrypi.local
```

Default password: ```raspberrypi```

#### Change password

It is recommended that you change the password.

```bash
passwd
```

This is especially important if the device is on a public network.


#### Configure Raspbian

Launch *raspi-config*:

```bash
sudo raspi-config
```

##### Enable SPI

Go to ```Interfacing Options```, and choose ```P4 SPI```.

Enable this option.


##### Expand file system

Go to ```Advanced```, and choose ```A1 Expand Filesystem [...]```.

A restart is required.

## Deploying app

Deploy a self-contained app by running a suitable ```publish``` script.

Modify the script with the credentials necessary to logon to your Raspberry Pi.

## Debugging app

You can debug the app from VS Code using the included launch configurations. You must probably install the Visual Studio Debugger also

Visual Studio 2019 supports Remote Debugging through SSH.

I have tested it without having the Visual Studio Debugger installed.

## External links

* [HEADLESS RASPBERRY PI 3 B+ SSH WIFI SETUP (MAC + WINDOWS)
](https://desertbot.io/blog/headless-raspberry-pi-3-bplus-ssh-wifi-setup)
* [Remote debugging with VS Code on Windows to a Raspberry Pi using .NET Core on ARM](https://www.hanselman.com/blog/RemoteDebuggingWithVSCodeOnWindowsToARaspberryPiUsingNETCoreOnARM.aspx)