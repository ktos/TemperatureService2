TemperatureAppService aka TemperatureService3
=============================================

[![Build Status](https://dev.azure.com/ktos/TemperatureService3/_apis/build/status/Master%20Build-Test-Dockerize?branchName=master)](https://dev.azure.com/ktos/TemperatureService3/_build/latest?definitionId=14&branchName=master)

One day many years ago I decided I want a "IoT" thermometer. I set up simple
solution with DS18B20, Raspberry Pi and PHP as a backend. Then I built some
clients to show current temperature - first for Windows Phone 8, later for
Windows 10, later Android, iOS... but also Google Glass, Microsoft Band, Pebble
and more. Just for fun, for every new platform or strange device I have I want a
TemperatureApp client.

This project is a .NET Core rewrite of my backend, saving sensors data in a
database and showing beautiful graphs, providing data for all strange clients.

Used mostly as a pretty cool experiment, allowing to tinker with ASP.NET Core
and similar tech.
