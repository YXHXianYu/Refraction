
public struct Result {
    public bool isSuc;
    public string errMsg;

    public Result(bool isSuc, string errMsg) {
        this.isSuc = isSuc;
        this.errMsg = errMsg;
    }
}