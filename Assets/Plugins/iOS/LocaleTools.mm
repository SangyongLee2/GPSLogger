#import "LocaleTools.h"

double latitude, longitude, altitude, timestamp;
float accuracy, verticalAccuracy, speed, speedAccuracy, heading, headingAccuracy;

@implementation LocaleTools

CLLocationManager *locationManager;

- (LocaleTools *)init
{
    
    printf("Location Start");
    
    
    locationManager = [[CLLocationManager alloc] init];
    locationManager.delegate = self;
    locationManager.distanceFilter = kCLDistanceFilterNone;
    locationManager.desiredAccuracy = kCLLocationAccuracyBest;
    locationManager.headingFilter = kCLHeadingFilterNone;
    
    if([[[UIDevice currentDevice] systemVersion] floatValue] >= 8.0)
        [locationManager requestWhenInUseAuthorization];

    [locationManager startUpdatingLocation];
    [locationManager startUpdatingHeading];

    return self;
}

- (void)stop
{
    printf("Location End");
    
    [locationManager stopUpdatingLocation];
    [locationManager stopUpdatingHeading];
    locationManager = NULL;
}

- (void)locationManager:(CLLocationManager *)manager didUpdateLocations:(NSArray *)locations;
{
    CLLocation *location = [locations lastObject];
    
    timestamp = location.timestamp.timeIntervalSince1970;

    latitude = location.coordinate.latitude;
    longitude = location.coordinate.longitude;
    accuracy = location.horizontalAccuracy;
    
    altitude = location.altitude;
    verticalAccuracy = location.verticalAccuracy;
    
    speed = location.speed;
    speedAccuracy = location.speedAccuracy;

}

- (void)locationManager:(CLLocationManager *) manager didUpdateHeading: (CLHeading *)headingV;
{
    heading = headingV.trueHeading;
    headingAccuracy = headingV.headingAccuracy;
}

- (bool) hasUserAuthorize
{
    CLAuthorizationStatus status = locationManager.authorizationStatus;
    
    switch (status)
    {
        case kCLAuthorizationStatusDenied:
        {
            return false;
        }
        case kCLAuthorizationStatusNotDetermined:
        {
            return false;
        }
        case kCLAuthorizationStatusRestricted:
        {
            return false;
        }
        default:
        {
            return true;
        }
    }
}


- (bool) isEnableGps
{
    return locationManager.locationServicesEnabled;
}

@end

static LocaleTools* localeDelegate = NULL;

extern "C"
{
    void startLocation()
    {
        if(localeDelegate == NULL) localeDelegate = [[LocaleTools alloc] init];
    }


    void stopLocation()
    {
        if ( localeDelegate == NULL ) { return; }
        
        [[LocaleTools alloc] stop];
        localeDelegate = NULL;
    }

    
    bool hasUserAuthorize()
    {
        if ( localeDelegate == NULL ) { return false;}
        
        return [[LocaleTools alloc] hasUserAuthorize];
    }

    bool isEnableGps()
    {
        if ( localeDelegate == NULL ) { return false;}
        return [[LocaleTools alloc] isEnableGps];
    }
                      

    double getTimestamp()
    {
        return timestamp;
    }


    double getLongitude()
    {
        return longitude;
    }

    double getLatitude()
    {
        return latitude;
    }

    double getAltitude()
    {
        return altitude;
    }

    float getAccuracy()
    {
        return accuracy;
    }

    float getVerticalAccuracyMeters()
    {
        return verticalAccuracy;
    }

    float getSpeed()
    {
        return speed;
    }

    float getSpeedAccuracy()
    {
        return speedAccuracy;
    }

    float getHeading()
    {
        return heading;
    }

    float getHeadingAccuracy()
    {
        return headingAccuracy;
    }

}

