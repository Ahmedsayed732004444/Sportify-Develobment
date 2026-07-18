namespace Sportiva.Enums;

public enum BookingStatus
{
    Pending,
    Confirmed,
    Rejected,   // ← جديد: صاحب الملعب رفض الطلب قبل التأكيد
    Cancelled,  // إلغاء بعد التأكيد (من العضو أو صاحب الملعب) أو إلغاء العضو لطلبه Pending
    Completed
}