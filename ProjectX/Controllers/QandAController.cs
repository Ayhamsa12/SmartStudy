using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Hup;
using ProjectX.Models;
using ProjectX.ViewModels;

namespace ProjectX.Controllers
{
    [Authorize]
    public class QandAController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public QandAController(UserManager<Users> userManager, ApplicationDbContext context, IHubContext<ChatHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> QandA()
        {
            var questions = await _context.UserQuestions
                .Include(q => q.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                .ToListAsync();

            var questionViewModels = questions.Select(q => new UserQuestionViewModel
            {
                Id = q.Id,
                UserId = q.UserId,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                User = q.User,
                Answers = q.Answers.Select(a => new UserAnswerViewModel
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    ReplyContent = a.ReplyContent,
                    CreatedAt = a.CreatedAt,
                    UserName = a.User.UserName
                }).ToList(),
                Score = q.Score
            }).ToList();

            return View(questionViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> RateQuestion(int questionId, bool isUpvote)
        {
            var user = await _userManager.GetUserAsync(User);

            var question = await _context.UserQuestions.FindAsync(questionId);

            if (question != null)
            {
                // Check if the user has already voted for this question
                var existingVote = _context.userQuestionVotes
                    .FirstOrDefault(qv => qv.QuestionId == questionId && qv.UserId == user.Id);

                if (existingVote != null)
                {
                    // User has already voted, check if the vote direction is the same
                    if (existingVote.IsUpvote == isUpvote)
                    {
                        // User is removing their vote
                        if (isUpvote)
                        {
                            question.Score--; // Decrease score for upvote removal
                        }
                        else
                        {
                            question.Score++; // Increase score for downvote removal
                        }

                        _context.userQuestionVotes.Remove(existingVote);
                    }
                    else
                    {
                        // User is switching their vote
                        if (isUpvote)
                        {
                            question.Score += 2; // Increase score for switching from downvote to upvote
                        }
                        else
                        {
                            question.Score -= 2; // Decrease score for switching from upvote to downvote
                        }

                        existingVote.IsUpvote = isUpvote;
                        _context.userQuestionVotes.Update(existingVote);
                    }
                }
                else
                {
                    // User is casting a new vote
                    var newVote = new UserQuestionVote
                    {
                        UserId = user.Id,
                        QuestionId = questionId,
                        IsUpvote = isUpvote
                    };

                    question.Score += isUpvote ? 1 : -1; // Adjust score based on new vote direction

                    _context.userQuestionVotes.Add(newVote);
                }

                await _context.SaveChangesAsync();
            }

            // Redirect back to the QandA page after voting
            return RedirectToAction(nameof(QandA));
        }

        public IActionResult ViewQuestion(int id)
        {
            var question = _context.UserQuestions
                .Include(q => q.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                .FirstOrDefault(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            var questionViewModel = new UserQuestionViewModel
            {
                Id = question.Id,
                UserId = question.UserId,
                Title = question.Title,
                Content = question.Content,
                CreatedAt = question.CreatedAt,
                User = question.User,
                Answers = question.Answers.Select(a => new UserAnswerViewModel
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    ReplyContent = a.ReplyContent,
                    CreatedAt = a.CreatedAt,
                    UserName = a.User.UserName
                }).ToList(),
                Score = question.Score
            };

            return View(questionViewModel);
        }

        [HttpGet]
        public IActionResult AskQuestion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AskQuestion(UserQuestion model)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                model.UserId = user.Id;
                model.CreatedAt = DateTime.Now;
                model.Score = 0; 

                _context.UserQuestions.Add(model);
                await _context.SaveChangesAsync();

                // Notify clients about the new question
                await _hubContext.Clients.All.SendAsync("ReceiveQuestion", model.Title, model.Content);

                return RedirectToAction(nameof(QandA));
            }

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> PostAnswer(int userQuestionId, string replyContent)
        {
            var user = await _userManager.GetUserAsync(User);
            var answer = new UserAnswer
            {
                UserQuestionId = userQuestionId,
                UserId = user.Id,
                ReplyContent = replyContent,
                CreatedAt = DateTime.Now,
            };

            _context.UserAnswers.Add(answer);
            await _context.SaveChangesAsync();

            // Notify clients about the new reply
            await _hubContext.Clients.All.SendAsync("ReceiveAnswer", userQuestionId, replyContent, user.UserName);

            var viewModel = new UserAnswerViewModel
            {
                Id = answer.Id,
                UserId = answer.UserId,
                ReplyContent = answer.ReplyContent,
                CreatedAt = answer.CreatedAt,
                UserName = user.UserName
            };

            return Json(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditQuestion(int id)
        {
            var question = await _context.UserQuestions
                .Include(q => q.User) 
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || question.UserId != currentUser.Id)
            {
                
                return RedirectToAction(nameof(QandA));
            }

            var model = new UserQuestionViewModel
            {
                Id = question.Id,
                UserId = question.UserId,
                Title = question.Title,
                Content = question.Content,
                CreatedAt = question.CreatedAt,
                User = question.User 
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditQuestion(UserQuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var question = await _context.UserQuestions.FindAsync(model.Id);
                if (question == null)
                {
                    return NotFound();
                }

                // Check if the current user is the owner of the question
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || question.UserId != currentUser.Id)
                {
                    // Redirect or show an error message
                    return RedirectToAction(nameof(QandA));
                }

                question.Title = model.Title;
                question.Content = model.Content;

                _context.UserQuestions.Update(question);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(QandA));
            }

            return View(model);
        }
        

        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.UserQuestions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || question.UserId != currentUser.Id)
            {
                // If the current user is not the owner, redirect or show an error message
                return RedirectToAction(nameof(QandA));
            }

            _context.UserQuestions.Remove(question);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(QandA));
        }
        [HttpGet]
        public async Task<IActionResult> GetAnswerContent(int answerId)
        {
            var answer = await _context.UserAnswers.FindAsync(answerId);
            return Json(new { content = answer.ReplyContent });
        }

        [HttpPost]
        public async Task<IActionResult> EditAnswer(int answerId, string editedContent)
        {
            var answer = await _context.UserAnswers.FindAsync(answerId);
            if (answer != null)
            {
                // Update the answer content
                answer.ReplyContent = editedContent;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            var answer = await _context.UserAnswers.FindAsync(id);

            if (answer == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || answer.UserId != currentUser.Id)
            {
                // Redirect or show an error message
                return RedirectToAction(nameof(QandA));
            }

            _context.UserAnswers.Remove(answer);
            await _context.SaveChangesAsync();

            // Redirect to the question details or wherever you want
            return RedirectToAction("ViewQuestion", "QandA", new { id = answer.UserQuestionId });
        }
        [HttpPost]
        public async Task<IActionResult> Search(string search)
        {
            var questions = await _context.UserQuestions
                .Include(q => q.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                .Where(q => q.Title.Contains(search) || q.Content.Contains(search))
                .ToListAsync();

            var questionViewModels = questions.Select(q => new UserQuestionViewModel
            {
                Id = q.Id,
                UserId = q.UserId,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                User = q.User,
                Answers = q.Answers.Select(a => new UserAnswerViewModel
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    ReplyContent = a.ReplyContent,
                    CreatedAt = a.CreatedAt,
                    UserName = a.User.UserName
                }).ToList()
            }).ToList();

            return View("QandA", questionViewModels);
        }
        [HttpPost]
        public async Task<IActionResult> SaveQuestion(int userQuestionId)
        {
            var user = await _userManager.GetUserAsync(User);

            // Check if the UserQuestionId exists in the UserQuestions table
            var userQuestion = await _context.UserQuestions.FindAsync(userQuestionId);

            if (userQuestion != null)
            {
                // Check if the question is already saved; if yes, unsave it
                var existingSavedQuestion = await _context.SavedQuestions
                    .Where(sq => sq.UserId == user.Id && sq.UserQuestionId == userQuestionId)
                    .FirstOrDefaultAsync();

                if (existingSavedQuestion != null)
                {
                    _context.SavedQuestions.Remove(existingSavedQuestion);
                    TempData["SavedQuestion"] = "Question Unsaved";
                }
                else
                {
                    // Save the question
                    var savedQuestion = new SavedQuestion
                    {
                        UserId = user.Id,
                        UserQuestionId = userQuestionId
                    };

                    _context.SavedQuestions.Add(savedQuestion);
                    TempData["SavedQuestion"] = "Question Saved";
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(QandA));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminDelete(int id)
        {
            var question = await _context.UserQuestions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.UserQuestions.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction("QandA");
        }

    }
}
