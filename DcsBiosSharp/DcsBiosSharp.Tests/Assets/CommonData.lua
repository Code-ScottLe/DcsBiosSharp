BIOS.protocol.beginModule("CommonData", 0x400)
BIOS.protocol.setExportModuleAircrafts(BIOS.ALL_PLAYABLE_AIRCRAFT)
local defineString = BIOS.util.defineString
local defineIntegerFromGetter = BIOS.util.defineIntegerFromGetter

local latDeg, latSec, latFractionalSec
local lonDeg, lonSec, lonFractionalSec
local altFt
local hdgDeg
local hdgDegFrac
local player
local ias
moduleBeingDefined.exportHooks[#moduleBeingDefined.exportHooks+1] = function()
	-- skip  this data if ownship export is disabled
	if not LoIsOwnshipExportAllowed() then return end

    player = LoGetPilotName()
	
	ias = LoGetIndicatedAirSpeed()
	iasEU = math.floor(ias * 0.36) * 10
	iasUS = ias * 1.94384449		-- knots
	_indicatedAirspeedEU = string.format("%4d", iasEU)
	_indicatedAirspeedUS = string.format("%4d", iasUS)
	
	local selfData = LoGetSelfData()
	if selfData.LatLongAlt == nil then return end
	altFt = selfData.LatLongAlt.Alt / 0.3048
	local lat = selfData.LatLongAlt.Lat
	local lon = selfData.LatLongAlt.Long
	
	latDeg = math.floor(lat)
	lat = lat - latDeg
	lat = lat * 60 -- convert to seconds
	latSec = math.floor(lat)
	lat = lat - latSec
	latFractionalSec = lat

	lonDeg = math.floor(lon)
	lon = lon - lonDeg
	lon = lon * 60 -- convert to seconds
	lonSec = math.floor(lon)
	lon = lon - lonSec
	lonFractionalSec = lon
	
	if selfData.Heading ~= nil then
		local hdgDegValue = selfData.Heading / (2 * math.pi) * 360
		hdgDeg = math.floor(hdgDegValue)
		hdgDegFrac = hdgDegValue - hdgDeg
	end
end

defineString("PILOTNAME", function()
		if not LoIsOwnshipExportAllowed() then return nil end
		return player .. string.char(0)
end, 24, "String", "Pilot Name")

defineIntegerFromGetter("LAT_DEG", function() return latDeg end, 59, "Position", "Latitude Degrees")
defineIntegerFromGetter("LAT_SEC", function() return latSec end, 59, "Position", "Latitude Seconds")
defineIntegerFromGetter("LAT_SEC_FRAC", function()
	if not LoIsOwnshipExportAllowed() then return nil end
	return math.floor(latFractionalSec*65535)
end, 65535, "Position", "Latitude Fractional Seconds (divide by 65535)")

defineString("IAS_EU", function() return _indicatedAirspeedEU .. string.char(0) end, 4, "Speed", "Indicated Airspeed KM H")
defineString("IAS_US", function() return _indicatedAirspeedUS .. string.char(0) end, 4, "Speed", "Indicated Airspeed KNT")
defineIntegerFromGetter("IAS_EU_INT", function()
	if not LoIsOwnshipExportAllowed() then return nil end
	return math.floor(ias * 0.36) * 10
end, 65535, "Speed", "Indicated Airspeed KM H (Int)")
defineIntegerFromGetter("IAS_US_INT", function()
	if not LoIsOwnshipExportAllowed() then return nil end
	return ias * 1.94384449
end, 65535, "Speed", "Indicated Airspeed KNT (Int)")



defineIntegerFromGetter("LON_DEG", function() return lonDeg end, 59, "Position", "Longitude Degrees")
defineIntegerFromGetter("LON_SEC", function() return lonSec end, 59, "Position", "Longitude Seconds")
defineIntegerFromGetter("LON_SEC_FRAC", function()
	if not LoIsOwnshipExportAllowed() then return nil end
	return math.floor(lonFractionalSec*65535)
end, 65535, "Position", "Longitude Fractional Seconds (divide by 65535)")

defineIntegerFromGetter("ALT_MSL_FT", function()
	if not LoIsOwnshipExportAllowed() then return nil end
	return math.floor(altFt)
end, 65535, "Altitude", "Altitude MSL (ft)")

defineIntegerFromGetter("HDG_DEG", function() return hdgDeg end, 360, "Heading", "Heading (Degrees)")
defineIntegerFromGetter("HDG_DEG_FRAC", function()
	if not LoIsOwnshipExportAllowed() then return nil end
	return hdgDegFrac * 127
end, 127, "Heading", "Heading (Fractional Degrees, divide by 127)")

BIOS.protocol.endModule()
