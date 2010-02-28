# Initializers
MONO_BASE_PATH = 
MONO_ADDINS_PATH =

# Install Paths
DEFAULT_INSTALL_DIR = $(pkglibdir)

# External libraries to link against, generated from configure
LINK_SYSTEM = -r:System
LINK_MONO_POSIX = -r:Mono.Posix
LINK_HYENA = -r:$(DIR_BIN)/Hyena.dll

DIR_BIN = $(top_builddir)/bin

# Core
REF_TRIPOD_CORE = $(LINK_MONO_POSIX) $(LINK_HYENA)
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

