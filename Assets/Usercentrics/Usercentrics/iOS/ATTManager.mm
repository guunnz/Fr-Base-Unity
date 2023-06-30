#import <Foundation/Foundation.h>
#import "TrackingTransparencyManager.h"


extern "C" {
NSUInteger InterfaceGetTrackingAuthorizationStatus() {
    if (@available(iOS 14, *)) {
        return [[TrackingTransparencyManager sharedInstance] getTrackingAuthorizationStatus];
    } else {
        return 0;
    }
}

void requestForAppTrackingTransparency(TrackingTransparencyDelegate delegate)
{
    if (@available(iOS 14, *)) {
        [[TrackingTransparencyManager sharedInstance] trackingAuthorizationRequest:delegate];
    } else {
        delegate(3);
    }
}
}
