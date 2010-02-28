BINARIES = \
	Hyena.dll \
	Hyena.dll.mdb \
	Hyena.Gui.dll \
	Hyena.Gui.dll.config \
	Hyena.Gui.dll.mdb

EXTRA_DIST = $(BINARIES)
CLEANFILES = $(addprefix $(top_builddir)/bin/, $(BINARIES))

install-data-local:
	mkdir -p $(pkglibdir); \
	for f in $(BINARIES); do \
		$(INSTALL) -m 644 $$f $(pkglibdir); \
	done

uninstall-local:
	for f in $(BINARIES); do \
		rm -f $(pkglibdir)/$$f; \
	done

all: $(BINARIES)
	mkdir -p $(top_builddir)/bin; \
	for f in $(BINARIES); do \
		cp $$f $(top_builddir)/bin/; \
	done

