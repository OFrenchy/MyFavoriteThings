SELECT B.FirstName as FollowerFirstName, AspNetUsers.Email, 
	a.FirstName as ContributorFirstName
	FROM 
		Contributors A inner join 
		Follows on A.ContributorID = Follows.ContributorID inner join
		Contributors B on Follows.FollowerContributorID = B.ContributorID inner join
		AspNetUsers on B.ApplicationUserId = AspNetUsers.Id 
		WHERE A.ContributorID = 1