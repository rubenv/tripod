SUBDIRS = \
    Mono.Data.Sqlite \
    Hyena \
    Hyena.Gui \
    SQLite \
    TagLib \
    UniqueSharp

### The stuff below takes care of embedding libraries. Build system hacks! ###

LIBRARIES_FROM_BANSHEE = \
    Hyena \
    Hyena.Gui \
    Mono.Data.Sqlite

TMPDIR_BANSHEE = /tmp/fetch_banshee
TMPDIR_TAGLIB = /tmp/fetch_taglib
TMPDIR_UNIQUE = /tmp/fetch_unique
TMPDIR_SQLITE = /tmp/fetch_sqlite

clone-banshee:
	mkdir $(TMPDIR_BANSHEE)
	git clone --depth 1 git://git.gnome.org/banshee $(TMPDIR_BANSHEE)

clone-taglib:
	mkdir $(TMPDIR_TAGLIB)
	git clone --depth 1 git://gitorious.org/taglib-sharp/mainline.git $(TMPDIR_TAGLIB)
	pushd $(TMPDIR_TAGLIB); git checkout -b photo-support origin/photo-support; popd

clone-uniquesharp:
	mkdir $(TMPDIR_UNIQUE)
	git clone --depth 1 git://gitorious.org/unique-sharp/unique-sharp.git $(TMPDIR_UNIQUE)

clone-sqlite:
	mkdir $(TMPDIR_SQLITE)
	pushd $(TMPDIR_SQLITE); svn checkout http://sqlite-net.googlecode.com/svn/trunk/src/ $(TMPDIR_SQLITE); popd

update-banshee:
	rm -rf $(LIBRARIES_FROM_BANSHEE)
	for l in $(LIBRARIES_FROM_BANSHEE); do \
		cp -r $(TMPDIR_BANSHEE)/src/Libraries/$$l .; \
	done
	
update-taglib:
	rm -rf TagLib/TagLib
	cp -r $(TMPDIR_TAGLIB)/src/TagLib TagLib
	
	echo "ASSEMBLY = TagLib" > TagLib/Makefile.am
	echo "TARGET = library" >> TagLib/Makefile.am
	echo "LINK = " >> TagLib/Makefile.am
	echo "SOURCES = \\" >> TagLib/Makefile.am
	tail -n+2 TagLib/TagLib/TagLib.sources | sed s/\$$\(srcdir\)\\/// >> TagLib/Makefile.am
	echo "include \$$(top_srcdir)/build/build.mk" >> TagLib/Makefile.am

update-uniquesharp:
	rm -rf UniqueSharp/generator/*.cs UniqueSharp/unique/{*.{custom,cs},unique-api.raw,Unique.metadata}
	cp $(TMPDIR_UNIQUE)/generator/*.cs UniqueSharp/generator/
	cp $(TMPDIR_UNIQUE)/unique/{*.{custom,cs},unique-api.raw,Unique.metadata} UniqueSharp/unique/

update-sqlite:
	rm -f SQLite/SQLite/SQLite.cs
	cp $(TMPDIR_SQLITE)/SQLite.cs SQLite/SQLite/

.PHONY: clone-banshee clone-taglib clone-uniquesharp clone-sqlite \
        update-banshee update-taglib update-uniquesharp update-sqlite
