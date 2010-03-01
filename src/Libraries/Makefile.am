SUBDIRS = \
    Mono.Data.Sqlite \
    Hyena

LIBRARIES_FROM_BANSHEE = \
    Hyena \
    Mono.Data.Sqlite

TMPDIR = /tmp/fetch_banshee

clone:
	mkdir $(TMPDIR)
	git clone --depth 1 git://git.gnome.org/banshee $(TMPDIR)

pull:
	pushd $(TMPDIR); \
	git pull; \
	popd;

update:
	rm -rf $(LIBRARIES_FROM_BANSHEE)
	for l in $(LIBRARIES_FROM_BANSHEE); do \
		cp -r $(TMPDIR)/src/Libraries/$$l .; \
	done

.PHONY: clone pull update
