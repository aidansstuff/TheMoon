namespace Steamworks
{
	internal enum AppOwnershipFlags
	{
		None = 0,
		OwnsLicense = 1,
		FreeLicense = 2,
		RegionRestricted = 4,
		LowViolence = 8,
		InvalidPlatform = 0x10,
		SharedLicense = 0x20,
		FreeWeekend = 0x40,
		RetailLicense = 0x80,
		LicenseLocked = 0x100,
		LicensePending = 0x200,
		LicenseExpired = 0x400,
		LicensePermanent = 0x800,
		LicenseRecurring = 0x1000,
		LicenseCanceled = 0x2000,
		AutoGrant = 0x4000,
		PendingGift = 0x8000,
		RentalNotActivated = 0x10000,
		Rental = 0x20000,
		SiteLicense = 0x40000,
		LegacyFreeSub = 0x80000,
		InvalidOSType = 0x100000
	}
}
