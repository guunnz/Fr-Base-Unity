# FriendBase
### Unity Application

Requirements
 - Git LFS
 - Android Build Support
 - iOS Build Support
 - WebGL Build Support

The Main Scene for play the game is Assets/Main Scene

### IOS Application Build

- Unity > Build settigs > IOS Target > Build
- Terminal > `cd Buidl-folder` > run if m1: `arch -x86_64 pod install` if intel: `pod install`
- Close Xcode > open `Unity-iPhone.xcworkspace` in Xcode
- Xcode > Signing & Capability >
  - Check Automaically manage signing
  - Team `Friendbase AB`
  - Bundle identifier: `com.friendbase.ios`
- Development build
  - Run `Start the active scheme`
- IPA Build
  - Product > Archive
  - Distribute App
  - Development
  - App Thinning: All compatible device vatiants
  - Check Automaically manage signing
  - Export