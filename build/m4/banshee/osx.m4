AC_DEFUN([BANSHEE_CHECK_OSX],
[
	enable_osx="no"
	if test "x${host_os%${host_os#??????}}" = "xdarwin"; then
		enable_osx="yes"
		PKG_CHECK_MODULES(IGEMACINTEGRATION, ige-mac-integration >= 0.8.6)
	fi
	AM_CONDITIONAL([PLATFORM_DARWIN], [test "x$enable_osx" = "xyes"])
])
