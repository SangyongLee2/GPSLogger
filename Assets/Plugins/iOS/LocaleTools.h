#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>
#import <UIKit/UIKit.h>

@interface LocaleTools : NSObject <CLLocationManagerDelegate>

- (LocaleTools *)init;
- (void)stop;
- (bool)hasUserAuthorize;
- (bool)isEnableGps;
- (void)locationManager:(CLLocationManager *)manager didUpdateLocations:(NSArray *)locations;
- (void)locationManager:(CLLocationManager *)manager didUpdateHeading:(CLHeading *)headingV;
@end
