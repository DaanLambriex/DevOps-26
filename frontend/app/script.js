console.log("script geladen");

document.getElementById("loadRepos").addEventListener("click", async () => {
    console.log("knop geklikt");

    const response = await fetch("http://localhost:5000/api/repositories");

    const repos = await response.json();

    const repoList = document.getElementById("repoList");

    repoList.innerHTML = "";

    repos.forEach(repo => {

        const li = document.createElement("li");

        li.textContent = `${repo.name} - ${repo.description ?? "Geen beschrijving"}`;

        repoList.appendChild(li);

    });

});