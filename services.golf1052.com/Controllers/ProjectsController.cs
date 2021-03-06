﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace services.golf1052.com.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        [HttpGet]
        public async Task<JArray> GetAll()
        {
            JArray projects = new JArray();
            List<Project> projectsList = new List<Project>();
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", Secrets.GitHubUsername);
            string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Secrets.GitHubUsername}:{Secrets.GitHubToken}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
            HttpResponseMessage repoResponse = await httpClient.GetAsync("https://api.github.com/user/repos?visibility=public&per_page=100");
            JArray repoResponseArray = JArray.Parse(await repoResponse.Content.ReadAsStringAsync());
            foreach (JObject repo in repoResponseArray)
            {
                string name = (string)repo["name"];
                string githubLink = (string)repo["html_url"]; 
                string affiliation = string.Empty;
                JObject owner = (JObject)repo["owner"];
                if ((string)owner["type"] == "User")
                {
                    if ((string)owner["login"] == Secrets.GitHubUsername)
                    {
                        affiliation = "owner";
                    }
                    else
                    {
                        affiliation = "collaborator";
                    }
                }
                else if ((string)owner["type"] == "Organization")
                {
                    affiliation = "organization";
                }
                else
                {
                    affiliation = "???";
                }
                DateTime created = DateTime.Parse((string)repo["created_at"]);
                DateTime updated = DateTime.Parse((string)repo["pushed_at"]);
                string projectLink = string.Empty;
                if (!string.IsNullOrEmpty((string)repo["homepage"]))
                {
                    projectLink = (string)repo["homepage"];
                }
                string description = string.Empty;
                if (!string.IsNullOrEmpty((string)repo["description"]))
                {
                    description = (string)repo["description"];
                }
                HttpResponseMessage languagesResponse = await httpClient.GetAsync($"https://api.github.com/repos/{(string)owner["login"]}/{(string)repo["name"]}/languages");
                JObject languagesResponseObject = JObject.Parse(await languagesResponse.Content.ReadAsStringAsync());
                List<string> languages = new List<string>();
                foreach (var language in languagesResponseObject)
                {
                    languages.Add(language.Key);
                }
                Project project = new Project(name, githubLink, affiliation, created, updated, projectLink, description, languages);
                projectsList.Add(project);
            }
            projectsList.Sort((p1, p2) => p1.Updated.CompareTo(p2.Updated) * -1);
            foreach (var project in projectsList)
            {
                projects.Add(project.ToJObject());
            }
            return projects;
        }
    }

    struct Project
    {
        public string Name { get; private set; }
        public string GithubLink { get; private set; }
        public string Affiliation { get; private set; }
        public DateTime Updated { get; private set; }
        public string Dates { get; private set; }
        public string ProjectLink { get; private set; }
        public string Description { get; private set; }
        public List<string> Languages { get; private set; }

        public Project(string name,
            string githubLink,
            string affiliation,
            DateTime created,
            DateTime updated,
            string projectLink,
            string description,
            List<string> languages)
        {
            Name = name;
            GithubLink = githubLink;
            Affiliation = affiliation;
            Updated = updated;
            Dates = $"{created.ToString("MMMM yyyy")} - {updated.ToString("MMMM yyyy")}";
            ProjectLink = projectLink;
            Description = description;
            Languages = languages;
        }

        public JObject ToJObject()
        {
            JObject o = new JObject();
            o["name"] = Name;
            o["github_link"] = GithubLink;
            o["affiliation"] = Affiliation;
            o["dates"] = Dates;
            o["project_link"] = ProjectLink;
            o["description"] = Description;
            o["languages"] = new JArray(Languages);
            return o;
        }
    }
}
