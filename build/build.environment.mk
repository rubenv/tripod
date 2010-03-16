# Initializers
MONO_BASE_PATH = 
MONO_ADDINS_PATH =

# Install Paths
DEFAULT_INSTALL_DIR = $(pkglibdir)

# External libraries to link against, generated from configure
LINK_SYSTEM = -r:System
LINK_MONO_POSIX = -r:Mono.Posix
LINK_CAIRO = -r:Mono.Cairo
LINK_GTK = $(GTKSHARP_LIBS)

DIR_BIN = $(top_builddir)/bin

# Mono.Data
REF_SQLITE = -r:System -r:System.Data -r:System.Transactions
LINK_SQLITE = -r:System.Data -r:$(DIR_BIN)/Mono.Data.Sqlite.dll

# Hyena
REF_HYENA = $(LINK_SYSTEM) $(LINK_SQLITE) $(LINK_MONO_POSIX)
LINK_HYENA = -r:$(DIR_BIN)/Hyena.dll
LINK_HYENA_DEPS = $(REF_HYENA) $(LINK_HYENA)

# Hyena.Gui
REF_HYENA_GUI = $(LINK_SYSTEM) $(LINK_SQLITE) $(LINK_MONO_POSIX) $(LINK_GTK) $(LINK_CAIRO) $(LINK_HYENA)
LINK_HYENA_GUI = -r:$(DIR_BIN)/Hyena.Gui.dll
LINK_HYENA_GUI_DEPS = $(REF_HYENA_GUI) $(LINK_HYENA_GUI)

# Core
REF_TRIPOD_CORE = $(LINK_MONO_POSIX) $(LINK_HYENA) $(LINK_GTK)
LINK_TRIPOD_CORE = -r:$(DIR_BIN)/Tripod.Core.dll
LINK_TRIPOD_CORE_DEPS = $(REF_TRIPOD_CORE) $(LINK_TRIPOD_CORE)

# Clients
REF_FLASHUNIT = $(LINK_TRIPOD_CORE_DEPS)

# Cute hack to replace a space with something
colon:= :
empty:=
space:= $(empty) $(empty)

# Build path to allow running uninstalled
RUN_PATH = $(subst $(space),$(colon), $(MONO_BASE_PATH))

