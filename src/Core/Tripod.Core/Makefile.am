ASSEMBLY = Tripod.Core
TARGET = library
LINK = $(REF_TRIPOD_CORE)
SOURCES =  \
	Tripod.Base/Core.cs \
	Tripod.Base/RecursiveDirectoryEnumerator.cs \
	Tripod.Base/TripodSqliteConnection.cs \
	Tripod.Jobs/ThreadPoolJob.cs \
	Tripod.Model/CachePhoto.cs \
	Tripod.Model/CachePhotoSource.cs \
	Tripod.Model/CachePhotoSourceFactory.cs \
	Tripod.Model/ICacheablePhotoSource.cs \
	Tripod.Model/INamingPolicy.cs \
	Tripod.Model/IPhoto.cs \
	Tripod.Model/IPhotoSource.cs \
	Tripod.Model/LocalFilePhoto.cs \
	Tripod.Model/LocalFolderPhotoSource.cs \
	Tripod.Model/MainCachePhotoSource.cs

RESOURCES =  \
	Resources/contributors.xml \
	Resources/COPYING

include $(top_srcdir)/build/build.mk

EXTRA_DIST += Tripod.Core.dll.config
module_SCRIPTS += Tripod.Core.dll.config
