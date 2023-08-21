public enum Protocol
{
    Test = 0,
    AddUser = 1,
}

public enum Error
{
    RuntimeException = -1,//서버 오류
    None = 0,//사용안함
    success = 200,//성공
    notFound = 404,//프로토콜 없음
    unknown = 500,//파라미터 오류
    crypto = 800,//암복호화 에러
    nodata = 900,//데이터가 없음
}