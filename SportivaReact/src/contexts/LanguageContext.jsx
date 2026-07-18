import React, { createContext, useState, useContext, useEffect } from 'react';

const LanguageContext = createContext();

const dictionary = {
  en: {
    // Nav & Sidebar
    home: 'Home',
    search: 'Search',
    courts: 'Search Courts',
    clubs: 'Clubs',
    exploreCourts: 'Explore Courts',
    exploreClubs: 'Explore Clubs',
    matches: 'Matches',
    friendlyMatches: 'Friendly Matches',
    tournaments: 'Tournaments',
    becomeOwner: 'Become Owner',
    signInJoin: 'Sign In / Join',
    marketplaceHome: 'Marketplace Home',
    dashboard: 'Dashboard',
    myDashboard: 'My Dashboard',
    bookings: 'Bookings',
    bookingsGrid: 'Bookings Grid',
    community: 'Community',
    communityFeed: 'Community Feed',
    messages: 'Messages',
    notifications: 'Notifications',
    profile: 'Profile',
    myProfile: 'My Profile',
    settings: 'Settings',
    logout: 'Logout',
    businessProfile: 'Business Profile',
    ownerDashboard: 'Owner Dashboard',
    subscription: 'Subscription',
    adminProfile: 'Admin Profile',
    adminDashboard: 'Admin Dashboard',
    backToMarketplace: 'Back to Marketplace',

    // Dashboard Common
    welcomeBack: 'Welcome Back',
    captain: 'Captain',
    dashboardDesc: 'Manage your booked courts, upcoming squads, and keep your athletic statistics active.',
    liveOperations: 'Live Operations',
    todaySchedule: 'Today\'s Schedule',
    manageVenue: 'Venue Management',
    activeTier: 'Active Tier',
    ownerRequests: 'Owner Requests',
    owners: 'Owners',
    players: 'Players',
    reports: 'System Reports',
    subscriptions: 'Subscriptions',
    moderation: 'Clubs Moderation',
    analytics: 'Analytics',

    // Player Profile View
    verifiedCaptain: 'Verified Captain',
    attendanceRate: 'Attendance Rate',
    completionRate: 'Completion Rate',
    cancellationRate: 'Cancellation Rate',
    sportsSkillLevel: 'Sports Skill Level',
    friendlyLobbies: 'Friendly Lobbies',
    tournamentRoster: 'Tournament Roster',
    preferredSport: 'Preferred Sport',
    earnedAchievements: 'Earned Achievements & Medals',
    captainReviews: 'Captain Reviews & Testimonials',
    identityReputation: 'Identity & Reputation',
    accountSettings: 'Account Settings',
    passwordSecurity: 'Password & Security',
    privacyNotifications: 'Privacy & Notifications',
    modifyIdentity: 'Modify Account Identity',
    firstName: 'First Name',
    lastName: 'Last Name',
    usernameHandle: 'Username Handle',
    cityLocation: 'City Location',
    favoriteSports: 'Favorite Sports',
    skillClassification: 'Skill Classification',
    athleticBioSummary: 'Athletic Bio Summary',
    saveChanges: 'Save Changes',
    sportsmanship: 'Sportsmanship',
    cityLabel: 'City',

    // Become Owner View
    choosePlan: 'Choose the Perfect Plan for Your Club Complex',
    choosePlanDesc: 'Scale your venue management, automate time slots generation, eliminate overlapping reservations, and gain exposure to thousands of players.',
    recommended: 'Recommended',
    perMonth: ' / month',
    cancelAnytime: 'Cancel or modify any time',
    maxClubsLabel: 'Max Clubs',
    maxCourtsLabel: 'Max Courts',
    activeCourts: 'Active Courts',
    timeSlotGen: 'Weekly Time-Slot Generator',
    peakRates: 'Dynamic Peak/Off-Peak Rates',
    selectPlanBtn: 'Select',
    fullVenueOwnership: 'Full Venue Ownership',
    fullVenueOwnershipDesc: 'Establish your business credentials, list unlimited sub-courts, and upload photos, amenities lists, and custom terms.',
    overlappingPrevention: 'Overlapping Prevention',
    overlappingPreventionDesc: 'A dynamic digital reservation engine prevents double booking by checking real-time slot statuses and pending requests.',
    automatedSchedules: 'Automated Schedules',
    automatedSchedulesDesc: 'Generate weekly bookings automatically using default open/close parameters, customize peak hours, and configure specific rates.',
    faqTitle: 'Frequently Asked Questions',
    faqSubtitle: 'Got questions about Sportify partnerships and billing? Here are answers to common questions.',
    faqQ1: 'How does the court booking commission work?',
    faqA1: 'Sportify setup is 100% free. We apply zero commissions on cash-on-field bookings. A minor gateway fee applies only if players pay online using visa or mobile wallets.',
    faqQ2: 'Can I set custom time slots and peak rates?',
    faqA2: 'Yes. Sportify includes a robust scheduling grid where you can set individual court availability and configure custom rates for evenings, weekends, or public holidays.',
    faqQ3: 'Can players find my club automatically?',
    faqA3: 'Once approved, your club is listed on our live search engine with direct booking links, interactive maps, photos, and live ratings.',
    faqQ4: 'How do I upgrade or renew my plan?',
    faqA4: 'Owners can visit their "Subscription" section from the sidebar and submit renewal or upgrade requests. A site admin will review details and approve manually.'
  },
  ar: {
    // Nav & Sidebar
    home: 'الرئيسية',
    search: 'البحث',
    courts: 'البحث عن الملاعب',
    clubs: 'الأندية',
    exploreCourts: 'استكشاف الملاعب',
    exploreClubs: 'استكشاف الأندية',
    matches: 'المباريات الودية',
    friendlyMatches: 'مباريات ودية',
    tournaments: 'البطولات',
    becomeOwner: 'شريك الملاعب',
    signInJoin: 'تسجيل الدخول / الانضمام',
    marketplaceHome: 'الرئيسية للمتجر',
    dashboard: 'لوحة التحكم',
    myDashboard: 'لوحتي الخاصة',
    bookings: 'حجوزاتي',
    bookingsGrid: 'شبكة الحجوزات',
    community: 'المجتمع الرياضي',
    communityFeed: 'آخر المشاركات',
    messages: 'الرسائل',
    notifications: 'الإشعارات',
    profile: 'الملف الشخصي',
    myProfile: 'ملفي الشخصي',
    settings: 'الإعدادات',
    logout: 'تسجيل الخروج',
    businessProfile: 'ملف العمل التجاري',
    ownerDashboard: 'لوحة تحكم المالك',
    subscription: 'الاشتراك الشهري',
    adminProfile: 'الملف الإداري',
    adminDashboard: 'لوحة تحكم المسؤول',
    backToMarketplace: 'العودة ',

    // Dashboard Common
    welcomeBack: 'مرحباً بعودتك',
    captain: 'كابتن',
    dashboardDesc: 'إدارة ملاعبك المحجوزة، فرقك القادمة، والحفاظ على نشاط إحصائياتك الرياضية.',
    liveOperations: 'العمليات المباشرة',
    todaySchedule: 'جدول اليوم',
    manageVenue: 'إدارة النادي',
    activeTier: 'الباقة النشطة',
    ownerRequests: 'طلبات الانضمام',
    owners: 'أصحاب الملاعب',
    players: 'اللاعبين',
    reports: 'تقارير النظام',
    subscriptions: 'الاشتراكات',
    moderation: 'رقابة النوادي',
    analytics: 'التحليلات والإحصاء',

    // Player Profile View
    verifiedCaptain: 'كابتن موثق',
    attendanceRate: 'نسبة الحضور',
    completionRate: 'نسبة إكمال المباريات',
    cancellationRate: 'نسبة الإلغاء',
    sportsSkillLevel: 'مستوى المهارة',
    friendlyLobbies: 'مباريات ودية',
    tournamentRoster: 'البطولات المشتركة',
    preferredSport: 'الرياضة المفضلة',
    earnedAchievements: 'الإنجازات والميداليات المكتسبة',
    captainReviews: 'تقييمات وآراء الكابتن',
    identityReputation: 'الهوية والتقييم',
    accountSettings: 'إعدادات الحساب',
    passwordSecurity: 'كلمة المرور والأمان',
    privacyNotifications: 'الخصوصية والإشعارات',
    modifyIdentity: 'تعديل معلومات الحساب',
    firstName: 'الاسم الأول',
    lastName: 'اسم العائلة',
    usernameHandle: 'اسم المستخدم',
    cityLocation: 'المدينة',
    favoriteSports: 'الرياضة المفضلة',
    skillClassification: 'تصنيف المهارة',
    athleticBioSummary: 'ملخص السيرة الذاتية الرياضية',
    saveChanges: 'حفظ التغييرات',
    sportsmanship: 'الروح الرياضية',
    cityLabel: 'المدينة',

    // Become Owner View
    choosePlan: 'اختر الخطة المثالية لمجمعك الرياضي',
    choosePlanDesc: 'طور إدارة ناديك، وأتمت إنشاء الفترات الزمنية، وتجنب الحجوزات المتداخلة، واحصل على وصول لآلاف اللاعبين.',
    recommended: 'موصى به',
    perMonth: ' / شهرياً',
    cancelAnytime: 'الإلغاء والتعديل متاح في أي وقت',
    maxClubsLabel: 'الحد الأقصى للأندية',
    maxCourtsLabel: 'الحد الأقصى للملاعب',
    activeCourts: 'ملاعب نشطة',
    timeSlotGen: 'مولد فترات زمنية أسبوعي',
    peakRates: 'أسعار ديناميكية للمناطق المزدحمة',
    selectPlanBtn: 'اختيار الخطة',
    fullVenueOwnership: 'ملكية كاملة للمنشأة',
    fullVenueOwnershipDesc: 'أنشئ هويتك التجارية، وأضف ملاعب فرعية غير محدودة، وارفع الصور وقائمة الخدمات والشروط الخاصة بك.',
    overlappingPrevention: 'منع الحجوزات المتداخلة',
    overlappingPreventionDesc: 'نظام حجز رقمي ديناميكي يمنع الازدواجية عبر التحقق من حالة الفترات الزمنية والطلبات المعلقة في الوقت الفعلي.',
    automatedSchedules: 'جداول زمنية مؤتمتة',
    automatedSchedulesDesc: 'أنشئ جداول الحجوزات الأسبوعية تلقائياً باستخدام إعدادات الفتح والإغلاق، وخصص ساعات الذروة وحدد أسعاراً مخصصة.',
    faqTitle: 'الأسئلة الشائعة',
    faqSubtitle: 'هل لديك أسئلة حول شراكات واشتراكات Sportify؟ إليك إجابات الأسئلة الأكثر شيوعاً.',
    faqQ1: 'كيف تعمل عمولة حجز الملاعب؟',
    faqA1: 'نظام Sportify مجاني 100٪. لا نفرض أي عمولات على الحجوزات المدفوعة نقداً في الملعب. تطبق رسوم بوابة دفع طفيفة فقط إذا دفع اللاعبون عبر الإنترنت باستخدام الفيزا أو المحافظ الإلكترونية.',
    faqQ2: 'هل يمكنني تحديد فترات زمنية وأسعار مخصصة للذروة؟',
    faqA2: 'نعم. يتضمن Sportify شبكة جدولة قوية حيث يمكنك تعيين إتاحة كل ملعب بشكل فردي وتكوين أسعار مخصصة للأمسيات أو عطلات نهاية الأسبوع أو العطلات الرسمية.',
    faqQ3: 'هل يمكن للاعبين العثور على ناديني تلقائياً؟',
    faqA3: 'بمجرد الموافقة، يتم إدراج ناديك في محرك البحث المباشر الخاص بنا مع روابط حجز مباشرة وخرائط تفاعلية وصور وتقييمات مباشرة.',
    faqQ4: 'كيف يمكنني ترقية أو تجديد خطتي؟',
    faqA4: 'يمكن للمالكين زيارة قسم "الاشتراك" من الشريط الجانبي وتقديم طلبات التجديد أو الترقية. سيقوم مسؤول الموقع بمراجعة التفاصيل والموافقة يدوياً.'
  }
};

export function LanguageProvider({ children }) {
  const [language, setLanguageState] = useState(() => localStorage.getItem('app_language') || 'ar');

  const setLanguage = (lang) => {
    setLanguageState(lang);
    localStorage.setItem('app_language', lang);
  };

  const isRtl = language === 'ar';

  useEffect(() => {
    // Force direction attribute on document body
    document.documentElement.dir = isRtl ? 'rtl' : 'ltr';
    document.documentElement.lang = language;
  }, [language, isRtl]);

  const t = (key) => {
    return dictionary[language]?.[key] || dictionary['en']?.[key] || key;
  };

  return (
    <LanguageContext.Provider value={{ language, setLanguage, isRtl, t }}>
      <div dir={isRtl ? 'rtl' : 'ltr'} className={`w-full min-h-screen ${isRtl ? 'font-sans' : ''}`}>
        {children}
      </div>
    </LanguageContext.Provider>
  );
}

export function useLanguage() {
  return useContext(LanguageContext);
}
