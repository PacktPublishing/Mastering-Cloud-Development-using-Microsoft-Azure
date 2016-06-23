WITH FailedLogins AS (
    SELECT a.UserId, COUNT(*) AS Attempts
    FROM Accounts AS a TIMESTAMP BY a.LocalTime
    JOIN LoginResults AS lr ON a.ResultId = lr.ResultId
    WHERE a.Category = 'Login' and lr.Result <> 'Success'
    GROUP BY HoppingWindow(Second, 600,10), a.UserId
    HAVING COUNT(*)> 3
)
SELECT 
    'Notification' AS Type,
    'Error' AS Severity,
    UserId,
    System.Timestamp AS LocalTime,
    ConCat('UserId ',
        CAST(UserId as nvarchar(max)),
        ' has failed login more than 3 times in the last 10 minutes') AS Message
INTO Feedback
FROM FailedLogins