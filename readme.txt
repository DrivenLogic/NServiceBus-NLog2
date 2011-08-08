A fork of nServiceBus 2.5 that seeks to support the following goals:

* Compiled against the latest Autofac 
* Compiled against latest NLog 2.0 build (Removing  common logging and log4net)

Note: Internally TopShelf dependencies still hold a reference to log4net
and Spring.core holds a reference to Common.logging