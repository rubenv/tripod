SUBDIRS = \
    generator \
    unique

ASSEMBLY = UniqueSharp
TARGET = library
LINK = $(GTKSHARP_LIBS)
SOURCES = \
    unique/AssemblyInfo.cs \
    unique/MessageReceivedHandler.cs \
    unique/generated/App.cs \
    unique/generated/Command.cs \
    unique/generated/MessageData.cs \
    unique/generated/Response.cs

include $(top_srcdir)/build/build.mk

EXTRA_DIST += UniqueSharp.dll.config
module_SCRIPTS += UniqueSharp.dll.config
