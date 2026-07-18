namespace Sportiva.Settings;

/// <summary>
/// إعدادات توليد الـ Time Slots — بتتقرأ من appsettings.json تحت السكشن "TimeSlot".
/// بدّلنا الـ consts اللي كانت مكتوبة جوه TimeSlotService لكلاس منفصل عشان تتغيّر
/// من غير ما نعمل Deploy لكود جديد.
/// </summary>
public class TimeSlotOptions
{
    public const string SectionName = "TimeSlot";

    /// <summary>ساعة بداية العمل (24-hour format). افتراضي 8 = 8:00 ص.</summary>
    public int OpeningHour { get; set; } = 8;

    /// <summary>
    /// ساعة نهاية العمل (24-hour format). 24 يعني لحد منتصف الليل.
    /// لاحظ إن TimeOnly مش بتقدر تمثل الساعة 24:00، فآخر Slot بينتهي بقيمة 00:00
    /// (يعني منتصف الليل / بداية اليوم اللي بعده) — ده تمثيل مقصود مش خطأ، راجع
    /// GenerateWeeklyTimeSlotsForCourtAsync للتفاصيل.
    /// </summary>
    public int ClosingHour { get; set; } = 24;

    /// <summary>مدة كل Slot بالساعات.</summary>
    public int SlotDurationHours { get; set; } = 1;

    /// <summary>عدد الأيام اللي بيتولد لها Slots مقدمًا في كل تشغيلة للـ Hangfire job.</summary>
    public int DaysToGenerate { get; set; } = 7;

    /// <summary>
    /// التايم زون المستخدم لتحديد "اليوم الحالي" وقت التوليد، بدل الاعتماد المباشر
    /// على DateTime.UtcNow اللي ممكن يدّي يوم غلط قريب من منتصف الليل بتوقيت مصر.
    /// جرّب الـ IANA ID الأول (بيشتغل على Linux/Docker)، ولو مش لاقيه يرجع لـ Windows ID.
    /// </summary>
    public string TimeZoneId { get; set; } = "Africa/Cairo";

    /// <summary>Fallback ID لو السيرفر شغال على Windows.</summary>
    public string WindowsTimeZoneId { get; set; } = "Egypt Standard Time";
}