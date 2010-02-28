# Embed Hyena binary. This sucks and it makes distcheck fail. Needs fixing.

hyenadir = $(libdir)
hyena_DATA = \
	Hyena.dll \
	Hyena.dll.mdb \
	Hyena.Gui.dll \
	Hyena.Gui.dll.config \
	Hyena.Gui.dll.mdb

EXTRA_DIST = $(hyena_DATA)
CLEANFILES = $(addprefix $(top_builddir)/bin/, $(hyena_DATA))

all: $(hyena_DATA)
	mkdir -p $(top_builddir)/bin; \
	for f in $(hyena_DATA); do \
		cp $$f $(top_builddir)/bin/; \
	done

