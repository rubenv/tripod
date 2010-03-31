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
	Tripod.Graphics/MipMapFile.cs \
	Tripod.Graphics/MipMapGenerator.cs \
	Tripod.Graphics/PixbufExtensions.cs \
	Tripod.Jobs/ThreadPoolJob.cs \
	Tripod.Model.Gui/IPhotoGridCaptionRenderer.cs \
	Tripod.Model.Gui/PhotoGridDateCaptionRender.cs \
	Tripod.Model.Gui/PhotoGridTextCaptionRenderer.cs \
	Tripod.Model.Gui/PhotoGridThumbnailCache.cs \
	Tripod.Model.Gui/PhotoGridView.cs \
	Tripod.Model.Gui/PhotoGridViewChild.cs \
	Tripod.Model.Gui/PhotoGridViewLayout.cs \
	Tripod.Model/CachePhoto.cs \
	Tripod.Model/CachePhotoSource.cs \
	Tripod.Model/IAcceptImportPhotoSource.cs \
	Tripod.Model/ICacheablePhotoSource.cs \
	Tripod.Model/ICachingPhotoSource.cs \
	Tripod.Model/INamingPolicy.cs \
	Tripod.Model/IPhoto.cs \
	Tripod.Model/IPhotoSource.cs \
	Tripod.Model/LocalFilePhoto.cs \
	Tripod.Model/LocalFolderNamingPolicy.cs \
	Tripod.Model/LocalFolderPhotoSource.cs \
	Tripod.Model/MainCachePhotoSource.cs

RESOURCES =  \
	Resources/contributors.xml \
	Resources/COPYING

include $(top_srcdir)/build/build.mk

EXTRA_DIST += Tripod.Core.dll.config
module_SCRIPTS += Tripod.Core.dll.config
