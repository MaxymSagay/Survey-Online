﻿using AutoMapper;
using Microsoft.AspNet.Identity;
using SurveyOnline.Core.Managers;
using SurveyOnline.DAL.Entities;
using SurveyOnline.WebSite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SurveyOnline.WebSite.Factories
{
    public class SurveyDetailVMFactory
    {
        /// <summary>
        /// Build survey detail view model from db.
        /// </summary>
        /// <returns>Survey veiw model by id.</returns>
        public SurveyDetailViewModel Build(int surveyID, bool isStatistic)
        {
            var surveyManager = new SurveyAPI();
            var questionManager = new QuestionAPI();
            var answerVariantManager = new AnswerVariantAPI();
            var questionnaireManager = new QuestionnaireAPI();

            var userID = HttpContext.Current.User.Identity.GetUserId();
            var surveyDB = surveyManager.GetSurveyIdByID(surveyID, userID);
            int questionCount = questionManager.GetCountQuestions(surveyID);
            var questionModel = questionManager.GetQuestions(surveyID);

            Mapper.Initialize(cfg => cfg.CreateMap<Question, SurveyQuestionsViewModel>());
            IEnumerable<SurveyQuestionsViewModel> questionVM = Mapper.Map<IEnumerable<Question>, IEnumerable<SurveyQuestionsViewModel>>(questionModel);

            foreach (var q in questionVM)
            {
                var ansverVariantModel = answerVariantManager.GetAnswerVariants(q.QuestionId);
                Mapper.Initialize(cfg => cfg.CreateMap<AnswerVariant, QuestionAnswersViewModel>());
                q.AnswersVariants = Mapper.Map<IEnumerable<AnswerVariant>, IEnumerable<QuestionAnswersViewModel>>(ansverVariantModel);
                if (isStatistic)
                {
                    foreach (var a in q.AnswersVariants)
                        a.AnswerVariantCount = questionnaireManager.GetAnswerCount(surveyID, a.AnswerVariantId);
                }
            }

            var vm = new SurveyDetailViewModel { SurveyName = surveyDB.SurveyName, SurveyDescription = surveyDB.SurveyDescription, SurveyStatus = surveyDB.SurveyStatus, SurvewyQuestionsCount = questionCount, SurveyQuestions = questionVM };
            return vm;
        }
    }
}