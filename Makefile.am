SUBDIRS = \
	build \
	src \
    extras \
	po

# Important targets

clean-local:
	rm -rf $(top_builddir)/bin

uninstall-hook:
	if test `find $(pkglibdir) | wc -l` -eq 1; then \
		rmdir $(pkglibdir); \
	fi

dist-hook:
	@( \
	  echo "`date +%Y-%m-%d`  The Tripod Project  <http://f-spot.org>"; \
	  echo; \
	  echo "@Tripod $(VERSION)"; \
	  echo; \
	  echo "@You probably are looking for the useful change summary detailed"; \
	  echo "@in the NEWS file, perfect for downstream maintainers."; \
	  echo; \
	  echo "@If you care about commit-level changes, use 'git log' against"; \
	  echo "@a clone of our git repository (git://git.gnome.org/TODO)."; \
	  echo; \
	) | tr '@' '\t' > $(top_distdir)/ChangeLog

# Developer targets

LC_BUILD = $(top_builddir)/bin/share/locale
PO_FILES = $(addprefix po/, $(addsuffix .gmo, $(ALL_LINGUAS)))
MO_FILES = $(patsubst po/%.gmo, $(LC_BUILD)/%/LC_MESSAGES/$(PACKAGE).mo, $(PO_FILES))

$(MO_FILES): $(LC_BUILD)/%/LC_MESSAGES/$(PACKAGE).mo: po/%.gmo
	$(mkdir_p) $(dir $@)
	cp -rf '$<' '$@'

check-bootstrap-env:
	@if test -z "$$BOCKBUILD_ENV"; then \
		for env in $$(find $(top_srcdir) -maxdepth 1 -name \*.env); do \
			echo "A bootstrap environment was found, but it does not"; \
			echo "appear to be sourced into your current environment:"; \
			echo; \
			echo "    $$env"; \
			echo; \
			echo "Source the environment first."; \
			echo; \
			exit 1; \
		done; \
	fi

run: check-bootstrap-env $(MO_FILES)
	(cd bin; $(MONO) --debug $(TRIPOD_DEV_MONO_OPTIONS) FlashUnit.exe --debug --uninstalled $(TRIPOD_DEV_OPTIONS))

run-logged: check-bootstrap-env $(MO_FILES)
	(cd bin; $(MONO) --debug $(TRIPOD_DEV_MONO_OPTIONS) FlashUnit.exe --debug --uninstalled $(TRIPOD_DEV_OPTIONS) | tee last-run-log)

run-perf: run-logged
	extras/perf-analyze-log bin/last-run-log

run-trace: $(MO_FILES)
	(cd bin; $(MONO) \
		--trace=$$(find . -name "*dll" | sed 's/\.\///' | sed 's/\.dll//' | sed 's/Mono.Data.Sqlite//' | sort | tr "\n" , | sed 's/,$$//') \
		FlashUnit.exe --uninstalled $(TRIPOD_DEV_OPTIONS) > ../tripod.trace)

run-profile-heap: $(MO_FILES)
	(cd bin; $(MONO) --profile=logging:heap=all,gc-dumps=-1 --debug $(TRIPOD_DEV_MONO_OPTIONS) FlashUnit.exe --debug --uninstalled $(TRIPOD_DEV_OPTIONS))

query-trace:
	@echo "Did you set TRIPOD_DEV_MONO_OPTIONS to --trace=disabled?"; \
	MONO_PID=$$(ps aux | grep mono | grep '\--uninstalled' | grep -v 'cd bin' | awk '{print$$2}'); \
	kill -USR2 $$MONO_PID

push-moblin:
	@extras/push-moblin

gdb:
	@pushd bin; \
	gdb mono --eval-command="handle SIGXCPU SIG35 SIGPWR nostop noprint" --eval-command="b g_return_if_fail_warning" --eval-command="r --debug FlashUnit.exe --debug --uninstalled"; \
	popd;

mdb:
	@pushd bin; \
	mdb -run FlashUnit.exe --debug --uninstalled $(TRIPOD_DEV_OPTIONS); \
	popd;

csharp:
	@pushd .; \
    echo ""; \
    echo "Starting csharp with Gtk#, Hyena, Tripod, etc referenced"; \
    csharp -lib:bin/ -pkg:glib-sharp-2.0 -pkg:gtk-sharp-2.0 -pkg:gconf-sharp-2.0 -pkg:dbus-sharp -pkg:taglib-sharp -pkg:ipod-sharp -pkg:mono-addins $(addprefix "-reference:", $(wildcard bin/*.dll)); \
	popd;

test:
	@pushd tests; \
	make test \
	popd;

hg:
	pushd bin; \
	echo "class Entry { static void Main () { Hyena.Gui.TestModuleRunner.Run (); } }" > tester.cs; \
	gmcs -r:Hyena.Gui.dll tester.cs; \
	mono --debug tester.exe; \
	rm tester.*; \
	popd; 

EXTRA_DIST = \
	intltool-extract.in \
	intltool-merge.in \
	intltool-update.in \
	AUTHORS \
	HACKING

CLEANFILES = \
	intltool-extract \
	intltool-update \
	intltool-merge

DISTCLEANFILES = \
	*.bak \
	*~ \
	*.pidb

MAINTAINERCLEANFILES = \
	compile \
	INSTALL \
	config.h.in \
	aclocal.m4 \
	ltmain.sh \
	Makefile.in \
	depcomp \
	missing \
	install-sh \
	configure \
	config.sub \
	config.guess \
	intltool-extract.in \
	intltool-merge.in   \
	intltool-update.in  \
	mkinstalldirs
