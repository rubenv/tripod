SRCS =								\
	$(srcdir)/AssemblyInfo.cs				\
	$(srcdir)/MessageReceivedHandler.cs

CUSTOMS =							\
	$(srcdir)/App.custom					\
	$(srcdir)/MessageData.custom	

unique-api.xml: $(srcdir)/unique-api.raw $(srcdir)/Unique.metadata
	@echo -e "\n*** Massaging the raw api into $@"
	cp $(srcdir)/unique-api.raw $(builddir)/unique-api.xml
	chmod +w unique-api.xml
	gapi2-fixup --api=unique-api.xml --metadata=$(srcdir)/Unique.metadata

$(builddir)/../generator/gapi_codegen.exe:
	$(MAKE) -C $(builddir)/../generator/

generated/App.cs: unique-api.xml $(CUSTOMS) $(builddir)/../generator/gapi_codegen.exe
	@echo -e "\n*** Generating C# code"
	$(MONO) $(builddir)/../generator/gapi_codegen.exe $(GLIBSHARP_CFLAGS) --outdir=generated --customdir=$(srcdir) --generate unique-api.xml

all: generated/App.cs

CLEANFILES = 			\
	unique-api.xml

DISTCLEANFILES =		\
	generated/*.cs

EXTRA_DIST =			\
	$(SRCS)			    \
	$(CUSTOMS)		    \
	Unique.metadata		\
	unique-api.raw
