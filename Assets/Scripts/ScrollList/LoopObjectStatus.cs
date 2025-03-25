namespace Game
{
    public enum LoopObjectStatus
    {
        NotShow, // 未创建
        UpOut, // 在现实区域外边(在上边)
        DownOut, // 在现实区域外边(在下边)
        Part, // 部分在现实区域里面
        Full, // 完全在现实区域里面
        LeftOut, // 在现实区域外边(在左边)
        RightOut // 在现实区域外边(在右边)
    }
}