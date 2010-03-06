SRCS =								\
	$(srcdir)/AssemblyInfo.cs				\
	$(srcdir)/FileFactory.cs                \
    $(srcdir)/GioStream.cs

CUSTOMS =							\
	$(srcdir)/AppInfoAdapter.custom					\
	$(srcdir)/FileAdapter.custom					\
	$(srcdir)/File.custom					\
	$(srcdir)/FileEnumerator.custom

gio-api.xml: $(srcdir)/gio-api-2.18.raw $(srcdir)/Gio.metadata
	@echo -e "\n*** Massaging the raw api into $@"
	cp $(srcdir)/gio-api-2.18.raw $(builddir)/gio-api.xml
	chmod +w gio-api.xml
	gapi2-fixup --api=gio-api.xml --metadata=$(srcdir)/Gio.metadata

$(builddir)/../generator/gapi_codegen.exe:
	$(MAKE) -C $(builddir)/../generator/

generated/AppInfoAdapter.cs: gio-api.xml $(CUSTOMS) $(builddir)/../generator/gapi_codegen.exe
	@echo -e "\n*** Generating C# code"
	$(MONO) $(builddir)/../generator/gapi_codegen.exe $(GLIBSHARP_CFLAGS) --outdir=generated --customdir=$(srcdir) --generate gio-api.xml

all: generated/AppInfoAdapter.cs

CLEANFILES = 			\
	gio-api.xml

DISTCLEANFILES =		\
	generated/*.cs

EXTRA_DIST =			\
	$(SRCS)			    \
	$(CUSTOMS)		    \
	Gio.metadata		\
	gio-api.raw
