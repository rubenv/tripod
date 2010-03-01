SUBDIRS = \
    Mono.Data.Sqlite \
    Hyena \
    TagLib

### The stuff below takes care of embedding libraries. Build system hacks! ###

LIBRARIES_FROM_BANSHEE = \
    Hyena \
    Mono.Data.Sqlite

TMPDIR_BANSHEE = /tmp/fetch_banshee
TMPDIR_TAGLIB = /tmp/fetch_taglib

clone-banshee:
	mkdir $(TMPDIR_BANSHEE)
	git clone --depth 1 git://git.gnome.org/banshee $(TMPDIR_BANSHEE)

clone-taglib:
	mkdir $(TMPDIR_TAGLIB)
	git clone --depth 1 git://gitorious.org/taglib-sharp/mainline.git $(TMPDIR_TAGLIB)
	pushd $(TMPDIR_TAGLIB); git checkout -b photo-support origin/photo-support; popd

pull:
	for d in $(TMPDIR_BANSHEE) $(TMPDIR_TAGLIB); do \
		pushd $$d; \
		git pull; \
		popd; \
	done

update:
	rm -rf $(LIBRARIES_FROM_BANSHEE)
	for l in $(LIBRARIES_FROM_BANSHEE); do \
		cp -r $(TMPDIR_BANSHEE)/src/Libraries/$$l .; \
	done
	rm -rf TagLib/TagLib
	cp -r $(TMPDIR_TAGLIB)/src/TagLib TagLib

.PHONY: clone-banshee clone-taglib pull update
