SUBDIRS = m4

GCONF_SCHEMA_EXTRACTOR_ASSEMBLY = gconf-schema-extractor.exe
TRANSLATOR_EXTRACTOR_ASSEMBLY = translator-extractor.exe
DLL_MAP_VERIFIER_ASSEMBLY = dll-map-verifier.exe
ADDIN_XML_STRING_EXTRACTOR_ASSEMBLY = addin-xml-string-extractor.exe

ALL_TARGETS = $(TRANSLATOR_EXTRACTOR_ASSEMBLY) $(DLL_MAP_VERIFIER_ASSEMBLY) $(ADDIN_XML_STRING_EXTRACTOR_ASSEMBLY)

if GCONF_SCHEMAS_INSTALL
bansheedir = $(pkglibdir)
banshee_SCRIPTS = $(GCONF_SCHEMA_EXTRACTOR_ASSEMBLY)
ALL_TARGETS += $(GCONF_SCHEMA_EXTRACTOR_ASSEMBLY)
$(GCONF_SCHEMA_EXTRACTOR_ASSEMBLY): GConfSchemaExtractor.cs
	$(MCS) -out:$@ $<
endif

all: $(ALL_TARGETS)

$(TRANSLATOR_EXTRACTOR_ASSEMBLY): TranslatorExtractor.cs
	if [ "x$(top_srcdir)" = "x$(top_builddir)" ]; then \
		$(MCS) -out:$@ $< && LC_ALL=en_US.UTF-8 $(MONO) $@ $(top_builddir)/po > \
			$(top_srcdir)/src/Core/Tripod.Core/Resources/translators.xml; \
	fi;

$(DLL_MAP_VERIFIER_ASSEMBLY): DllMapVerifier.cs
	$(MCS) -out:$@ $<

$(ADDIN_XML_STRING_EXTRACTOR_ASSEMBLY): AddinXmlStringExtractor.cs
	$(MCS) -r:System.Xml -out:$@ $<

EXTRA_DIST = \
	icon-theme-installer \
	private-icon-theme-installer \
	GConfSchemaExtractor.cs \
	TranslatorExtractor.cs \
	DllMapVerifier.cs \
	AddinXmlStringExtractor.cs \
	dll-map-makefile-verifier \
	gconf-schema-rules

CLEANFILES = *.exe *.mdb
MAINTAINERCLEANFILES = Makefile.in

