using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Reddit;
using Reddit.Controllers;

namespace RedditStatisticsApp
{
    class Program
    {
        private const string RedditClientId = "GBfG-j8ak04XRS03w4johQ";
        private const string RedditClientSecret = "1oW85yRtFS6fXEZ887QNUdXf0Gu8Ew";
        private const string RedditUsername = "YOUR_REDDIT_USERNAME";
        private const string RedditPassword = "YOUR_REDDIT_PASSWORD";
        private const string SubredditName = "programming"; // Change this to your chosen subreddit

        private const string accessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlNIQTI1NjpzS3dsMnlsV0VtMjVmcXhwTU40cWY4MXE2OWFFdWFyMnpLMUdhVGxjdWNZIiwidHlwIjoiSldUIn0.eyJzdWIiOiJ1c2VyIiwiZXhwIjoxNzE3MDg4MzkxLjA3MTU4NSwiaWF0IjoxNzE3MDAxOTkxLjA3MTU4NSwianRpIjoiN1NfdFdtdlY5YlBKYTZ0SF9INWNIb2d5ZmJRSnpnIiwiY2lkIjoiR0JmRy1qOGFrMDRYUlMwM3c0am9oUSIsImxpZCI6InQyXzExODljcGVxdG4iLCJhaWQiOiJ0Ml8xMTg5Y3BlcXRuIiwibGNhIjoxNzE2ODA0MTc3OTg1LCJzY3AiOiJlSnlLVnRKU2lnVUVBQURfX3dOekFTYyIsImZsbyI6OX0.S1b5b2WXbrrIr2PrBhGT6-_ZT2_glIzcWdP18UW0EMjK3q4HCaAWe0TJGTACkR2QsMo-FMjLSKCW7e3XvJCjcwXzROqNCokFvm7p6v2DYfszro3L3Gse1Y0H_iSVdpI041Hu9xs81KG5HnVzcaA8EnEChURqE253J54ZwoF5DfFWNkPCnBi55ezjPgVXt_hM7l9y6dr_be_M_oO2IlCTs2Ctm78WrmUQNz0udvPJHgsmlBBkvyJLfS3ujp_0ZqYT9kV2BsNhc3wLVdNzm97Hg7GnTCsQk35MDD9SNusTa3yjVAKBccc5s1u3-7DsD-277Y8RLbUka5CfziO8m_QBrg";
        private const string refreshToken = null;
        private const string userAgent = "Client/0.1 by Numerous_Practice872"; 



        static async Task Main(string[] args)
        {
        
           var reddit = new RedditClient(RedditClientId, refreshToken, RedditClientSecret, accessToken, userAgent); 
            
            var subreddit = reddit.Subreddit(SubredditName);
            

            // Start listening to new posts
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var task = Task.Run(() => ListenToPosts(subreddit, cancellationToken));


            Console.ReadLine();
            await task;
        
            }

        static async Task ListenToPosts(Reddit.Controllers.Subreddit subreddit, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    
                    SubredditPosts posts = subreddit.Posts;
                    List<Post> topPosts = subreddit.Posts.GetTop("All").ToList();
                   
                    var mostUpvotedPost = topPosts.OrderByDescending(p=>p.Score).FirstOrDefault();
                    var usersWithMostPosts = topPosts.GroupBy(p=> p.Author).OrderByDescending(g=>g.Count()).FirstOrDefault();


                    //// Report statistics
                    if(mostUpvotedPost != null && usersWithMostPosts != null)
                    {
                    ReportStatistics(mostUpvotedPost.Title, mostUpvotedPost.Score, usersWithMostPosts.Key, usersWithMostPosts.Count());
                    }


                    await Task.Delay(TimeSpan.FromSeconds(30)); // Delay to avoid exceeding rate limit
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static void ReportStatistics(string mostUpvotedPostTitle, int mostUpvotedPostScore, string userWithMostPosts, int mostPostsCount)
        {
            Console.WriteLine("Reporting statistics...");
            Console.WriteLine($"Most upvoted post: {mostUpvotedPostTitle} ({mostUpvotedPostScore} upvotes)");
            Console.WriteLine($"User with most posts: {userWithMostPosts} ({mostPostsCount} posts)");
        }

    }
}
