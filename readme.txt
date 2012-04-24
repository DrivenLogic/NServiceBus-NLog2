
A fork of nServiceBus 2.6 that seeks to support the following goals:

* Removal of common logging and log4net (replaced with NLog 2)
* Compiled against Autofac 2.5.2.830-NET40
* Compiled against NLog v2.0.0.2000 (.net 4.0)

Note: Internally TopShelf dependencies still hold a reference to log4net
and Spring.core holds a reference to Common.logging

