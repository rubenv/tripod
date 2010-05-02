ASSEMBLY = Tripod.Core
TARGET = library
LINK = $(REF_TRIPOD_CORE)
SOURCES =  \
	Tripod.Actions/ChangeCommentBatchPhotoAction.cs \
	Tripod.Actions/ChangeCommentPhotoAction.cs \
	Tripod.Actions/IBatchPhotoAction.cs \
	Tripod.Actions/IPhotoAction.cs \
	Tripod.Base/Core.cs \
	Tripod.Base/GIOTagLibFileAbstraction.cs \
	Tripod.Base/RecursiveDirectoryEnumerator.cs \
	Tripod.Base/TripodSqliteConnection.cs \
	Tripod.Graphics/ImageSurfaceCache.cs \
	Tripod.Graphics/IPhotoLoader.cs \
	Tripod.Graphics/MipMapFile.cs \
	Tripod.Graphics/MipMapGenerator.cs \
	Tripod.Graphics/MipMappedPhotoLoader.cs \
	Tripod.Graphics/PhotoLoaderCache.cs \
	Tripod.Graphics/PixbufExtensions.cs \
	Tripod.Jobs/ThreadPoolJob.cs \
	Tripod.Model/EnumerableListModel.cs \
	Tripod.Model/HyenaLINQModel.cs \
	Tripod.Model/PhotoModelFactory.cs \
	Tripod.Model/TripodQuery.cs \
	Tripod.Sources.SqliteCache/SqliteCachedPhoto.cs \
	Tripod.Sources.SqliteCache/SqliteCachedPhotoSource.cs \
	Tripod.Sources.SqliteCache/SqlitePhotoSourceCache.cs \
	Tripod.Sources/BasePhotoSourceInfo.cs \
	Tripod.Sources/IAcceptImportPhotoSource.cs \
	Tripod.Sources/ICacheablePhotoSource.cs \
	Tripod.Sources/IDbCacheablePhoto.cs \
	Tripod.Sources/IImportablePhotoSource.cs \
	Tripod.Sources/INamingPolicy.cs \
	Tripod.Sources/IPhoto.cs \
	Tripod.Sources/IPhotoSource.cs \
	Tripod.Sources/IPhotoSourceCache.cs \
	Tripod.Sources/IPhotoSourceInfo.cs \
	Tripod.Sources/PhotoSourceInfoManager.cs \
	Tripod.Sources/PhotoSourceNotAvailableException.cs \
	Tripod.Tasks/CancellableTask.cs \
	Tripod.Tasks/ChildCancellableTask.cs \
	Tripod.Tasks/RefCountCancellableTask.cs

RESOURCES =  \
	Resources/contributors.xml \
	Resources/COPYING \
	Tripod.Core.addin.xml \
	Tripod.Core.addins

module_DATA = Tripod.Core.addins

include $(top_srcdir)/build/build.mk

EXTRA_DIST += Tripod.Core.dll.config
module_SCRIPTS += Tripod.Core.dll.config
