# KakaoTalkAdBlockPlus 개발 계획 (TDD)

`D:\Sources\KakaoTalkAdBlock-2.1.4`(Go)를 분석해 C#(.NET Framework 4.8, WPF)으로 다시 만든다.

- 광고 제거 기능은 원본과 같다. 다만 설치된 카카오톡 26.8.1에서 동작하도록,
  같은 원본 저장소의 최신판(2.2.4)이 쓰는 **일반(Win32) 클라이언트용 규칙**을 옮긴다.
  (2.1.4의 `BannerAdWnd`/`BannerAdContainer`/`AdFitWebView` 규칙은 원본에서도 이미 폐기됨)
- 바뀌는 점: 트레이 **우클릭 메뉴(설정/종료)**, **설정창**(확인 주기, 윈도우 시작 시 자동 실행),
  재부팅 후에도 유지되는 설정 저장.

## 작업 규칙

- "go" → 아래에서 체크되지 않은 **다음 테스트 하나**를 작성 → 실패 확인(레드) →
  통과시키는 최소 코드(그린) → 필요하면 구조 정리(리팩터) → 매번 전체 테스트 실행.
- 구조 변경과 행위 변경은 커밋을 나눈다. 커밋 메시지에 `[구조]` / `[행위]`를 붙인다.
- 테스트 실행: `powershell -ExecutionPolicy Bypass -File build.ps1` (빌드 + 전체 테스트)

## 테스트 목록

### 1. 확인 주기 — `CheckInterval`
- [x] 기본값은 원본과 같은 100ms이다 — `ShouldDefaultToOriginalHundredMilliseconds`
- [x] 50ms보다 짧으면 50ms로 보정한다 — `ShouldClampBelowMinimumToFiftyMilliseconds`
- [x] 60초보다 길면 60초로 보정한다 — `ShouldClampAboveMaximumToSixtySeconds`
- [x] 초 단위 문자열로 표시한다 (100ms→"0.1", 1500ms→"1.5") — `ShouldFormatAsSecondsText`
- [x] 초 단위 문자열을 해석한다 ("0.25"→250ms) — `ShouldParseSecondsText`
- [x] 쉼표 소수점, "초"/"s" 단위, 앞뒤 공백을 허용한다 — `ShouldParseCommaDecimalAndUnitSuffix`
- [x] 빈 문자열은 `Empty`로 거부한다 — `ShouldRejectEmptyText`
- [x] 숫자가 아니면 `NotANumber`로 거부한다 — `ShouldRejectNonNumericText`
- [x] 최소보다 작으면 `TooSmall`로 거부한다 — `ShouldRejectTooSmallText`
- [x] 최대보다 크면 `TooLarge`로 거부한다 — `ShouldRejectTooLargeText`

### 2. 슬라이더 단계 — `IntervalSteps`
- [x] 인덱스로 단계 값을 얻는다 (0→50ms, 1→100ms, 마지막→60초) — `ShouldReturnStepValueForIndex`
- [x] 값에 가장 가까운 단계를 찾는다 — `ShouldFindNearestStepIndex`

### 3. 설정 저장 — `JsonSettingsStore`
- [x] 파일이 없으면 기본 설정을 읽는다 — `ShouldLoadDefaultsWhenFileDoesNotExist`
- [x] 저장한 확인 주기를 다시 읽는다 — `ShouldRoundTripCheckInterval`
- [x] 폴더가 없어도 만들어서 저장한다 — `ShouldCreateDirectoryWhenSaving`
- [x] 파일이 손상됐으면 기본 설정을 읽는다 — `ShouldLoadDefaultsWhenFileIsCorrupted`
- [x] 범위를 벗어난 값은 보정해서 읽는다 — `ShouldClampOutOfRangeIntervalWhenLoading`
- [x] 값이 빠져 있으면 기본값을 쓴다 — `ShouldUseDefaultIntervalWhenFieldIsMissing`
- [x] 다시 저장하면 이전 값을 덮어쓰고 다른 파일은 만들지 않는다 — `ShouldOverwritePreviousSettings`
  (원자적 저장은 하지 않음: 파일이 깨져도 기본값으로 읽는 테스트가 있어 충분)

### 4. 윈도우 시작 시 자동 실행 — `StartupRegistration` (가짜 레지스트리)
- [x] Run 값이 없으면 꺼져 있다 — `ShouldBeDisabledWhenRunValueIsMissing`
- [x] 켜면 `"실행파일 경로" --autostart`를 기록한다 — `ShouldWriteQuotedPathWithAutostartArgumentWhenEnabled`
- [x] 켠 뒤에는 켜져 있다 — `ShouldBeEnabledAfterEnable`
- [x] 끄면 Run 값을 지운다 — `ShouldDeleteRunValueWhenDisabled`
- [x] 작업 관리자에서 "사용 안 함"이면 꺼져 있다 — `ShouldBeDisabledWhenTaskManagerDisabledIt`
- [x] 작업 관리자에서 "사용"으로 표시돼 있으면 켜져 있다 — `ShouldStayEnabledWhenTaskManagerEnabledIt`
- [x] 켜면 작업 관리자의 "사용 안 함" 표시를 지운다 — `ShouldClearTaskManagerFlagWhenEnabled`
- [x] 켜져 있는데 실행 파일이 옮겨졌으면 현재 경로로 고친다 — `ShouldRepairCommandWhenExecutableMoved`
- [x] 꺼져 있으면 고치지 않는다 — `ShouldNotRepairWhenDisabled`

### 5. 실제 레지스트리 — `CurrentUserRegistryStore` (HKCU 임시 키)
- [x] 문자열 값을 쓰고, 읽고, 지운다 — `ShouldWriteReadAndDeleteStringValue`
- [x] 없는 키나 값은 null이다 — `ShouldReturnNullForMissingValue`
- [x] 이진 값을 읽는다 — `ShouldReadBinaryValue`

### 6. 명령줄 — `CommandLineOptions`
- [x] `--autostart`를 대소문자 구분 없이 알아본다 — `ShouldDetectAutostartFlag`
- [x] 인자가 없으면 직접 실행한 것이다 — `ShouldTreatNoArgumentsAsManualStart`

### 7. 광고 제거 엔진 — `AdBlockEngine` (가짜 창 트리)
- [x] 카카오톡 프로세스가 없으면 "실행 안 함"으로 보고한다 — `ShouldReportNotRunningWhenNoKakaoTalkProcess`
- [x] 메인 창의 이름 없는 `EVA_ChildWindow`(배너)에 WM_CLOSE를 보낸다 — `ShouldCloseUnnamedBannerChildOfMainWindow`
- [x] 메인 창의 첫 번째 자식은 닫지 않는다 — `ShouldNotCloseFirstChildOfMainWindow`
- [x] 다른 프로세스의 창은 건드리지 않는다 — `ShouldIgnoreWindowsOfOtherProcesses`
- [x] `OnlineMainView`/`LockModeView`가 없는 창(동영상 플레이어 등, 원본 #99)은 건드리지 않는다 — `ShouldIgnoreWindowWithoutMainOrLockView`
- [x] `_EVA_` 클래스 자손이 있는 자식(이모티콘 화면 등, 원본 #97)은 닫지 않는다 — `ShouldNotCloseChildContainingCustomScroll`
- [x] 직계 자식이 아닌 창은 닫지 않는다 — `ShouldNotCloseNestedDescendants`
- [x] 이름 있는 자식은 닫지 않는다 — `ShouldNotCloseNamedChildren`
- [ ] 제목이 없거나 소유자가 있는 창은 메인 창이 아니다 — `ShouldIgnoreUntitledOrOwnedWindows`
- [ ] `OnlineMainView`를 (메인 폭-2, 높이-31)로 늘려 배너 자리를 덮는다 — `ShouldResizeOnlineMainViewOverBannerArea`
- [ ] 창이 너무 작으면 `OnlineMainView` 크기 조정을 건너뛴다 — `ShouldSkipOnlineMainViewResizeWhenWindowTooSmall`
- [ ] `LockModeView`를 (메인 폭-2, 높이)로 맞춘다 — `ShouldResizeLockModeViewToFullHeight`
- [ ] `Chrome Legacy Window`를 품은 이름 없는 `EVA_Window` 팝업을 숨긴다 — `ShouldHidePopupAdContainingChromeLegacyWindow`
- [ ] 메인 창이 소유한 이름 없는 `EVA_Window_Dblclk` 팝업을 숨긴다 — `ShouldHideOwnedPopupAdOfMainWindow`
- [ ] `Chrome Legacy Window`가 없으면 숨기지 않는다 — `ShouldNotHidePopupWithoutChromeLegacyWindow`
- [ ] 이미 숨겨진 팝업은 다시 숨기지 않는다 — `ShouldNotHideAlreadyHiddenPopup`
- [ ] 제거한 광고 창 목록을 보고한다 — `ShouldReportRemovedAds`

### 8. 카카오톡 프로세스 찾기
- [ ] 캐시: 처음 호출하면 실제로 조회한다 — `ShouldQueryInnerSourceOnFirstCall`
- [ ] 캐시: 갱신 주기 안에서는 결과를 재사용한다 — `ShouldReuseIdsWithinRefreshPeriod`
- [ ] 캐시: 갱신 주기가 지나면 다시 조회한다 — `ShouldRefreshAfterRefreshPeriod`
- [ ] Toolhelp: 실행 파일 이름(대소문자 무시)으로 현재 프로세스를 찾는다 — `ShouldFindCurrentProcessByImageName`
- [ ] Toolhelp: 없는 이름이면 빈 목록이다 — `ShouldReturnEmptyForUnknownImageName`

### 9. Win32 창 API — `Win32WindowApi` (테스트 프로세스 안의 진짜 창)
- [ ] 최상위 창과 그 프로세스 ID를 얻는다 — `ShouldListTopLevelWindowWithProcessId`
- [ ] 클래스 이름, 텍스트, 부모를 읽는다 — `ShouldReadClassNameTextAndParent`
- [ ] 자손 창을 전위 순서로 얻는다 — `ShouldListDescendantsInPreOrder`
- [ ] 창을 닫는다 (WM_CLOSE) — `ShouldCloseWindow`
- [ ] 창을 숨기고 표시 여부를 읽는다 — `ShouldHideWindow`
- [ ] 창 크기를 바꾼다 — `ShouldResizeWindow`
- [ ] 엔진 + 실제 창: 카카오톡 구조를 흉내 낸 창에서 배너를 제거한다 — `ShouldRemoveBannerFromSimulatedKakaoTalkWindow`

### 10. 상태 집계 — `AdBlockStatusTracker`
- [ ] 처음에는 카카오톡 미실행, 제거 0개다 — `ShouldStartWithNothingRemoved`
- [ ] 보고서를 반영해 실행 여부와 제거 수를 누적한다 — `ShouldAccumulateRemovedAds`
- [ ] 같은 창은 한 번만 센다 — `ShouldCountSameWindowOnce`

### 11. 상태 문구 — `StatusText`
- [ ] 카카오톡이 없으면 "카카오톡 실행을 기다리는 중" — `ShouldDescribeWaitingForKakaoTalk`
- [ ] 카카오톡이 있으면 제거 수와 함께 "광고를 차단하고 있어요" — `ShouldDescribeBlockingWithCount`

### 12. 백그라운드 실행 — `AdBlockService`
- [ ] 시작하면 주기마다 엔진을 실행한다 — `ShouldRunEngineRepeatedlyAfterStart`
- [ ] 멈추면 더 실행하지 않는다 — `ShouldStopRunningAfterStop`
- [ ] 주기를 바꾸면 이전 대기를 끝내고 바로 반영한다 — `ShouldApplyNewIntervalWithoutWaitingForOldOne`
- [ ] 엔진에서 예외가 나도 계속 실행한다 — `ShouldKeepRunningWhenEngineThrows`
- [ ] 엔진 보고서를 상태에 반영한다 — `ShouldPublishStatusFromEngineReports`

### 13. 중복 실행 방지 — `SingleInstanceGuard`
- [ ] 처음 실행하면 첫 인스턴스다 — `ShouldBeFirstInstanceWhenNameIsFree`
- [ ] 이미 실행 중이면 첫 인스턴스가 아니다 — `ShouldNotBeFirstInstanceWhenAlreadyRunning`
- [ ] 두 번째 실행이 알리면 첫 인스턴스가 신호를 받는다(설정창 열기) — `ShouldSignalFirstInstanceWhenSecondStarts`
- [ ] 해제하면 다시 첫 인스턴스가 될 수 있다 — `ShouldReleaseNameOnDispose`

### 14. 설정창 뷰모델 — `SettingsViewModel`
- [ ] 현재 주기를 초 단위로 보여 준다 — `ShouldShowCurrentIntervalInSeconds`
- [ ] 올바른 입력은 바로 적용하고 저장한다 — `ShouldApplyAndSaveValidIntervalText`
- [ ] 숫자가 아닌 입력은 오류를 보여 주고 적용하지 않는다 — `ShouldShowErrorForInvalidIntervalText`
- [ ] 범위를 벗어난 입력은 허용 범위를 알려 준다 — `ShouldShowRangeErrorForOutOfRangeText`
- [ ] 슬라이더 단계를 고르면 적용·저장하고 입력칸도 바뀐다 — `ShouldApplySliderStep`
- [ ] 직접 입력한 값에 가장 가까운 단계로 슬라이더가 움직인다 — `ShouldMoveSliderToNearestStepWhenTextApplied`
- [ ] 기본값으로 되돌린다 — `ShouldResetIntervalToDefault`
- [ ] 자동 실행 상태를 보여 준다 — `ShouldReflectStartupRegistration`
- [ ] 스위치로 자동 실행을 켜고 끈다 — `ShouldToggleStartupRegistration`
- [ ] 레지스트리 오류가 나면 스위치를 되돌리고 오류를 보여 준다 — `ShouldRevertStartupToggleWhenRegistryFails`
- [ ] 상태 문구를 새로 고친다 — `ShouldRefreshStatusText`

## 테스트가 아닌 작업 (테스트 목록 완료 후)
- [ ] 아이콘 (XAML 벡터 → 다중 크기 .ico 생성 스크립트)
- [ ] 트레이 아이콘 + 우클릭 메뉴(설정/종료), 더블클릭 시 설정창
- [ ] 설정창 UI (라이트/다크 테마)
- [ ] 앱 조립(Program/App): 중복 실행 방지, 자동 실행 경로 복구, 첫 실행 안내 풍선
- [ ] build.ps1 (빌드 + 테스트 + dist 복사), README
