#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>
#import <UIKit/UIKit.h>

@interface LocaleTools : NSObject <CLLocationManagerDelegate>

- (LocaleTools *)initialize;
- (void)startLocation;
- (void)stop;
- (void)destroy;
- (bool)hasUserAuthorize;
- (bool)isEnableGps;
- (void)locationManager:(CLLocationManager *)manager didUpdateLocations:(NSArray *)locations;
- (void)locationManager:(CLLocationManager *)manager didUpdateHeading:(CLHeading *)headingV;
@end
