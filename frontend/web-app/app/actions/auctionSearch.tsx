'use server'
import { Item, PagedResult } from "@/types";
import { error } from "console";

export async function searchData(queryString:string):Promise<PagedResult<Item>>{
    const res = await fetch(`http://localhost:3000/search/${queryString}`)
    if (!res.ok) throw new Error('Search failed!');
    return res.json();
}